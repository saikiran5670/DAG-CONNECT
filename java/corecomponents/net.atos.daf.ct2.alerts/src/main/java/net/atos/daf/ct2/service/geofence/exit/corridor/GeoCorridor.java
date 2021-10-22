// Project:	DAF Connect 2.0
// File:	net.atos.ct2.geo / GeoCorridor.java
// Version:	1.0
//
// History:
//	version 	date         	by    	description
//	======= 	============ 	===== 	===========
//	1.0 		Oct, 2021	 	andl 	first version
//

package net.atos.daf.ct2.service.geofence.exit.corridor;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.util.Comparator;
import java.util.Objects;
import java.util.TreeSet;

/**
 * Simple Geo corridor implementation.
 * @author Andl
 */
public class GeoCorridor {

	/** The logger to use. */
	private static final Logger log = LogManager.getLogger(GeoCorridor.class);
	
	/** Corridor route points. */
	private final TreeSet<RoutePoint> points = new TreeSet<>(RoutePoint.COMPARATOR);	// NOPMD
	
	/** First route point. */
	private final RoutePoint startPoint;

	/** Last route point. */
	//private final RoutePoint endPoint;

	/** Corridor width in kilometers. */
	private final double width;
	
	/** Bounding-box coordinate latitude minimum. */
	private final double minLatitude;

	/** Bounding-box coordinate longitude minimum. */
	private final double minLongitude;
	
	/** Bounding-box coordinate latitude maximum. */
	private final double maxLatitude;

	/** Bounding-box coordinate longitude maximum. */
	private final double maxLongitude;

	/** Strategy to find closest route point, 1 = simple, 2 = ordering. */
	private int strategy = ORDERING;

	/** Constant for simple strategy. */
	public static final int SIMPLE = 1;
	
	/** Constant for ordering strategy. */
	public static final int ORDERING = 2;
	
	/**
	 * Instantiate geo corridor.
	 * @param route <code>double[][]</code> containing latitude and longitude in second dimension
	 * @param wdth The width of the corridor 
	 */
	public GeoCorridor(double[][] route, double wdth) {
	
		if (null == route) {
			throw new IllegalArgumentException("route[][2] cannot be null");
		}

		final int NR_PARAMS = 2;
		
		// bounding box
		double minLat = Double.MAX_VALUE;
		double minLng = Double.MAX_VALUE;
		double maxLat = Double.MIN_VALUE;
		double maxLng = Double.MIN_VALUE;
		
		// populate next and previous pointers
		RoutePoint cur = null;
		RoutePoint prv = null;
		RoutePoint start = null;
		int step = 1;

		for (double[] point: route) {
			if (NR_PARAMS == point.length) {
				double lat = point[0];
				double lng = point[1];

				points.add(cur = new RoutePoint(step++, lat, lng));	// NOPMD
				
				if (null == prv) {
					start = cur;
				} else {
					cur.setPrevious(prv);
					prv.setNext(cur);
				}
				prv = cur;
				
				// track bounding box
				minLat = (minLat < lat) ? minLat : lat;
				maxLat = (maxLat > lat) ? maxLat : lat;
				minLng = (minLng < lng) ? minLng : lng;
				maxLng = (maxLng > lng) ? maxLng : lng;
			}
		}

		width = wdth;
		startPoint = start;
		//endPoint = cur;

		// combine width into bounding box 
		double latitudeDistanceToDegrees = GPSLocation.latitudeDistanceToDegrees(wdth);
		minLatitude = minLat - latitudeDistanceToDegrees;
		maxLatitude = maxLat + latitudeDistanceToDegrees;

		double longitudeDistanceToDegrees = GPSLocation.longitudeDistanceToDegrees(minLat, wdth);
		minLongitude = minLng - longitudeDistanceToDegrees;
		maxLongitude = maxLng + longitudeDistanceToDegrees;

		if (log.isDebugEnabled()) {
			log.debug("Created corridor " + toString());
		}
	}

	/**
	 * Retrieve this corridors' width.
	 * @return the width
	 */
	public double getWidth() {
		return width;
	}

	/**
	 * Retrieve this corridors' strategy.
	 * @return the strategy
	 */
	public int getStrategy() {
		return strategy;
	}

	/**
	 * Set this corridors' strategy.
	 * @param strategy the strategy to set
	 */
	public void setStrategy(int strategy) {
		this.strategy = strategy;
	}

	/**
	 * Check whether location is inside the corridor.
	 * @param lat	latitude
	 * @param lng	longitude
	 * @return <code>true</code> if given coordinates fall within the corridor.
	 */
	public boolean liesWithin(double lat, double lng) {
		
		return liesWithin(new RoutePoint(lat, lng));
	}
	
	/**
	 * Naive check of location is inside the corridor.
	 * Currently based on circular distance to route points.
	 * @param location RoutePoint to check against this corridor.
	 * @return <code>true</code> if given location falls within the corridor.
	 */
	public boolean liesWithin(RoutePoint location) {
		
		boolean result = false;

		if (null == location) {
			// nothing to check
			if (log.isDebugEnabled()) {
				log.debug("liesWithin: location == null");
			}

		} else if (minLatitude > location.latitude || minLongitude > location.longitude ||
					maxLatitude < location.latitude || maxLongitude < location.longitude) {
			// outside of the bounding box
			if (log.isDebugEnabled()) {
				log.debug("liesWithin: Outside bounding box:\n\tlat=" + location.latitude + ",min=" + minLatitude + ",max=" + maxLatitude +
														"\n\tlng=" + location.longitude + ",min=" + minLongitude + ",max=" + maxLongitude);
			}
		} else {

			switch (strategy) {
			default: /* FALLTHROUGH */
			case SIMPLE:
				result = simpleLiesWithin(location);
				break;
			case ORDERING:
				result = orderLiesWithin(location);
				break;
			}
		}
		return result;
	}

	/**
	 * Checks whether <code>location</code> is within <code>width</code> distance of <code>point</code>. 
	 * @param location RoutePoint representing a location
	 * @param point RoutePoint within the corridors' route.
	 * @return <code>true</code> if given points are closer than width.
	 */
	private boolean liesWithin(RoutePoint location, RoutePoint point) {

		boolean result = false;
		
		if (null != location && null != point) {
			
			double dist = location.gpsDistance(point);
			
			result = (width >= dist);
		}
		
		return result;
	}

	/**
	 * Simple implementation, iterates through the collection.
	 * @param location The location to check.
	 * @return boolean
	 */
	private boolean simpleLiesWithin(RoutePoint location) {
		boolean result = false;
		int steps = 0;	// Informational, can be removed.

		double 		distance = Double.MAX_VALUE;
		RoutePoint 	closest = null;

		// iterate the whole route, 
		//   quit when within circle distance on one of the points
		//   remember the closest route point
		for (RoutePoint point = startPoint; null != point; point = point.getNext()) {
			double dist = location.gpsDistance(point);
			steps++;

			if (width >= dist) {
				result = true;
				break;
			}
			
			if (dist < distance) {
				distance = dist;
				closest = point;
			}
		}

		if (!result && null != closest) {

			// check rectangle
			if (log.isDebugEnabled()) {
				log.debug("!within dist=" + distance + "\n\t  loc=" + location + 
						"\n\tclose=" + closest +
						"\n\t prev=" + closest.getPrevious() + "\n\t next=" + closest.getNext());
			}

			// check within route section between closest point and previous/next  
			result = liesWithinSection(location, closest, distance);
		}

		if (log.isDebugEnabled()) {
			log.debug("simpleLiesWithin: result=" + result + " steps=" + steps + "/" + points.size());
		}

		return result;
	}

	/**
	 * Implementation using ordering, uses Tree search to locate closest route point.
	 * @param location The location to check.
	 * @return boolean
	 */
	private boolean orderLiesWithin(RoutePoint location) {
		boolean result;

		// Start with closest point based on ordering
		RoutePoint point = points.floor(location);

		if (null == point) {
			point = points.ceiling(location);
		}

		// Are we done?
		double distance = location.gpsDistance(point);
		int steps = 1;	// Informational, can be removed.

		result = (width >= distance);

		// If not done search for closest point based on ordering
		if (!result && null != point) {
			RoutePoint closest = point;
			RoutePoint lower = point;
			RoutePoint higher = point;

			// determine max order difference for the search
			double odlat = location.latitude  + GPSLocation.latitudeDistanceToDegrees(width);
			double odlng = location.longitude + GPSLocation.longitudeDistanceToDegrees(odlat, width);
			double odmax = 3.3 * Math.abs(location.order - RoutePoint.orderFormula(odlat, odlng));
					
			if (log.isDebugEnabled()) {
				log.debug("searching odmax=" + odmax + " loc=" + location);
			}

			double odlow = 0;
			double odhigh = 0;
			double dist = 0;

			// look further up & down until found or the ordering difference becomes too large
			while (!result && ((odlow  < odmax && null != lower) ||
							   (odhigh < odmax && null != higher))) {

				// look down, remember closest
				if (odlow  < odmax && null != lower && null != (lower  = points.lower(lower))) {	// NOPMD
					steps++;
					odlow = Math.abs(point.order - lower.order);
					dist = location.gpsDistance(lower);

					if (log.isTraceEnabled()) {
						log.trace("step:" + steps + " lower od=" + odlow + " dist=" + distance + " alt=" + lower);
					}
					
					if (width >= dist) {
						result = true;
						break;
					}
					
					if (dist < distance) {
						distance = dist;
						closest = lower;
					}
				}
				
				// look up, remember closest
				if (odhigh  < odmax && null != higher && null != (higher  = points.higher(higher))) {	// NOPMD
					steps++;
					odhigh = Math.abs(point.order - higher.order);
					dist = location.gpsDistance(higher);

					if (log.isTraceEnabled()) {
						log.trace("step:" + steps + " highr od=" + odhigh + " dist=" + distance + " alt=" + higher);
					}
					
					if (width >= dist) {
						result = true;
						break;
					}
					
					if (dist < distance) {
						distance = dist;
						closest = higher;
					}
				}
			}

			// check within route section between closest point and previous/next  
			if (!result) {
				if (log.isDebugEnabled()) {
					log.debug("!within dist=" + distance + "\n\t  loc=" + location + 
							"\n\tclose=" + closest +
							"\n\t prev=" + closest.getPrevious() + "\n\t next=" + closest.getNext());
				}

				result = liesWithinSection(location, closest, distance);
			}
		}

		if (log.isDebugEnabled()) {
			log.debug("orderLiesWithin: result=" + result + " steps=" + steps + "/" + points.size());
		}

		return result;
	}

	/**
	 * Checks whether <code>location</code> is within the route section near <code>routePoint</code>. 
	 * @param location <code>RoutePoint</code> representing a location
	 * @param routePoint closest <code>RoutePoint</code> to <code>location</code>.
	 * @param distance the distance between <code>RoutePoint</code> and <code>location</code>.
	 * @return <code>true</code> if the location lies inside the nearest route section.
	 */
	private boolean liesWithinSection(RoutePoint location, RoutePoint routePoint, double distance) {
		RoutePoint sectionStart = routePoint;
		double distToStart = distance;
		RoutePoint sectionEnd = sectionStart.getNext();
		double distToEnd = location.gpsDistance(sectionEnd);
		RoutePoint sectionAlt = sectionStart.getPrevious();
		double distToAlt = location.gpsDistance(sectionAlt);
		
		// determine section to check, closest - next or previous - closest
		if (distToAlt < distToEnd) {
			sectionEnd = sectionStart;
			sectionStart = sectionAlt;
			distToEnd = distToStart;
			distToStart = distToAlt;
		}

		// determine point where section line crosses height line from location:
		// length of section line
		double sectionLength = sectionStart.getSectionLength();

		// length of section line until cross point 
		double crossLength = (distToStart * distToStart - distToEnd * distToEnd) / (2 * sectionLength) + sectionLength / 2;
		
		// cross point
		GPSLocation cross = new GPSLocation(sectionStart.latitude + (sectionEnd.latitude - sectionStart.latitude) * crossLength / sectionLength, 
											sectionStart.longitude + (sectionEnd.longitude - sectionStart.longitude) * crossLength / sectionLength);

		// check distance between location and cross point
		double distToCross = location.gpsDistance(cross); 

		boolean result = (width >= distToCross);

		if (log.isDebugEnabled()) {
			log.debug("liesWithinSection: result=" + result + " dist=" + distToCross + " loc=" + location + "cross=" + cross);
		}

		return result;
	}

	@Override
	public final String toString() {
		return "GeoCorridor [points=" + points.size() + ", width=" + width + ", startPoint=" + startPoint + "]";
	}

	/**
	 * Corridor route point. 
	 * @author Andl
	 */
	private static final class RoutePoint extends GPSLocation {

		/** Order represents ordering between coordinates. */
		private final double order;

		/** Preserve route point sequence to next point. */
		private RoutePoint next = null;
		
		/** Preserve route point sequence to previous point. */
		private RoutePoint previous = null;

		/** Length of the route section between this route point and the next. */
		private double sectionLength = -1;

		/** Order represents ordering between coordinates. (Informational, can be removed.) */
		private final int step;

		/** GeoCoordinate comparator. */
		public static final RoutePointComparator COMPARATOR = new RoutePointComparator();

		/**
		 * Instantiate a route point coordinate.
		 * @param lat The latitude.
		 * @param lng The longitude.
		 */
		public RoutePoint(double lat, double lng) {
			this(Integer.MIN_VALUE, lat, lng);
		}
		
		/**
		 * Instantiate a route point coordinate.
		 * @param nr The step number of this point in the route.
		 * @param lat The latitude.
		 * @param lng The longitude.
		 */
		public RoutePoint(int nr, double lat, double lng) {
			super(lat, lng);
			step = nr;

			order = orderFormula(lat, lng);
		}

		/**
		 * Calculate order from latitude and longitude.
		 * @param lat latitude
		 * @param lng longitude
		 */
		private static double orderFormula(double lat, double lng) {
			// multiple order algorithms seem to work
			//return lat * lat + lng * lng;
			//return lng * 90 + lat;
			return lng + lat;
		}

		/**
		 * Retrieve next point in route or <code>null</code>.
		 * @return the next
		 */
		public RoutePoint getNext() {
			return next;
		}

		/**
		 * Set the next point in the route.
		 * @param next the next to set
		 */
		public void setNext(RoutePoint next) {
			this.next = next;
		}

		/**
		 * Retrieve previous point in route or <code>null</code>.
		 * @return the previous
		 */
		public RoutePoint getPrevious() {
			return previous;
		}

		/**
		 * Set the previous point in the route.
		 * @param previous the previous to set
		 */
		public void setPrevious(RoutePoint previous) {
			this.previous = previous;
		}

		/**
		 * Retrieve the length between this point and the next.
		 * @return the section length or <code>NaN</code> for the last point in the route
		 * @see #getNext()
		 */
		public double getSectionLength() {
			// calculate on-demand
			if (0 > sectionLength) {
				sectionLength = (null == next) ? Double.NaN : gpsDistance(next);
			}
			return sectionLength;
		}

		@Override
		public int hashCode() {
			final int prime = 31;
			int result = super.hashCode();
			result = prime * result + Objects.hash(order);
			return result;
		}

		@Override
		public boolean equals(Object obj) {
			if (this == obj) {
				return true;
			}
			if (!super.equals(obj)) {
				return false;
			}
			if (getClass() != obj.getClass()) {
				return false;
			}
			RoutePoint other = (RoutePoint) obj;
			return super.equals(obj) &&
					step == other.step;
		}

		@Override
		public String toString() {
			return "RoutePoint [" + ((Integer.MIN_VALUE == step) ? "" : step + ":") + 
					" latitude=" + latitude + ", longitude=" + longitude + ", order=" + order + "]";
		}

		/**
		 * Compare RoutePoints, first based on order then on longitude and latitude.
		 * @see Comparable#compareTo(Object)
		 */
		@Override
		public int compareTo(GPSLocation other) {
			RoutePoint that = (RoutePoint) other;
			
			int result = Double.compare(this.order, that.order);

			if (0 == result) {
				result = Double.compare(this.longitude, that.longitude);
			}

			if (0 == result) {
				result = Double.compare(this.latitude, that.latitude);
			}

			return result;
		}

		/**
		 * Compare geo coordinates on distance from 0,0.
		 */
		private static final class RoutePointComparator implements Comparator<RoutePoint> {

			@Override
			public int compare(RoutePoint o1, RoutePoint o2) {

				return o1.compareTo(o2);
			}
		}
	}
}
