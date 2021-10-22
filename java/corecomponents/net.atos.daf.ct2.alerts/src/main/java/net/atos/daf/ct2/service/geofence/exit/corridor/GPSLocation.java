// Project:	DAF Connect 2.0
// File:	net.atos.ct2.geo / GPSLocation.java
// Version:	1.0
//
// History:
//	version 	date         	by    	description
//	======= 	============ 	===== 	===========
//	1.0 		Oct, 2021	 	andl 	first version
//

package net.atos.daf.ct2.service.geofence.exit.corridor;

import java.util.Comparator;
import java.util.Objects;

/**
 * Simple GPS location data holder.
 * @author Andl
 */
public class GPSLocation implements Comparable<GPSLocation> {

	/** The logger to use. */
	//private static final Logger log = LogManager.getLogger(GPSLocation.class);
	
	/** Latitude. */
	protected final double	latitude;

	/** Longitude. */
	protected final double	longitude;

	/** GeoCoordinate comparator. */
	public static final GPSLocationComparator COMPARATOR = new GPSLocationComparator();

	/** Earth's radius in kilometers. */
	public static final double EARTH_RADIUS = 6371;

	/** Distance per degree latitude in kilometers. */
	private static final double KILOMETERS_PER_DEGREE_LATITUDE = 111.19;

	/**
	 * Instantiate a geo coordinate.
	 * @param lat The latitude.
	 * @param lng The longitude.
	 */
	public GPSLocation(double lat, double lng) {
		latitude = lat;
		longitude = lng;
	}

	/**
	 * Get this locations' latitude.
	 * @return double the latitude
	 */
	public double getLatitude() {
		return latitude;
	}

	/**
	 * Get this locations' longitude.
	 * @return double the longitude
	 */
	public double getLongitude() {
		return longitude;
	}

	/**
	 * Calculate distance to other GPS location in kilometers.
	 * @param that GPSLocation to calculate distance to.
	 * @return double GPS distance or <code>Double.NaN</code> if that is null.
	 * @see GPSLocation#gpsDistance(double, double, double, double)
	 */
	public double gpsDistance(GPSLocation that) {

	    return (null == that) ? Double.MAX_VALUE : gpsDistance(that.latitude, that.longitude);
	}
	
	/**
	 * Calculate distance to other GPS location in kilometers.
	 * @param lat The latitude.
	 * @param lng The longitude.
	 * @see GPSLocation#gpsDistance(double, double, double, double)
	 */
	public double gpsDistance(double lat, double lng) {

		return gpsDistance(latitude, longitude, lat, lng);
	}
	
	/**
	 * Calculate distance to other GPS location in kilometers.
	 * Uses the Haversine algorithm.
	 * @param lat1 The latitude of point.
	 * @param lng1 The longitude of point.
	 * @param lat2 The latitude of location.
	 * @param lng2 The longitude of location.
	 */
	public static double gpsDistance(double lat1, double lng1, double lat2, double lng2) {

	    double sdlat = Math.sin(Math.toRadians(lat2 - lat1) / 2);
	    double sdlon = Math.sin(Math.toRadians(lng2 - lng1) / 2);
	    double q = sdlat * sdlat + Math.cos(Math.toRadians(lat2)) * Math.cos(Math.toRadians(lat1)) * sdlon * sdlon;
	    double result = 2 * EARTH_RADIUS * Math.asin(Math.sqrt(q));

	    return result;
	}
	
	/**
	 * Calculate degrees latitude based on distance traveled.
	 * Uses the fact that latitude distance does not vary with latitude. 
	 * The result is slightly higher than theoretically correct.  
	 * @param distance distance in kilometer
	 * @return double degrees latitude covering the distance given.
	 */
	public static double latitudeDistanceToDegrees(double distance) {
		
		return distance / KILOMETERS_PER_DEGREE_LATITUDE;
	}
	
	/**
	 * Calculate degrees longitude based on distance traveled at a given latitude.
	 * Uses a lookup table with distances per degree longitude per latitude. 
	 * The result is slightly higher than theoretically correct.  
	 * @param lat the latitude at which we want to know the longitude degrees
	 * @param distance distance in kilometer
	 * @return double degrees longitude covering the distance given, <code>0</code> for invalid input.
	 */
	public static double longitudeDistanceToDegrees(double lat, double distance) {
		// alternatively, use a static table to to this lookup
		//		/** Distance per degree longitude in kilometers per degree latitude. */
		//		private static final double[] KILOMETERS_PER_DEGREE_LONGITUDE = { <static initialiser> };

		//		int index = (int)Math.abs(lat + 0.9);
		//		
		//		return (KILOMETERS_PER_DEGREE_LONGITUDE.length > index) ? distance / KILOMETERS_PER_DEGREE_LONGITUDE[index] : 0;
	
		return distance / gpsDistance(lat, 0, lat, 1);
	}
	
	@Override
	public int hashCode() {
		return Objects.hash(latitude, longitude);
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj) {
			return true;
		}
		if (null == obj || getClass() != obj.getClass()) {
			return false;
		}
		GPSLocation other = (GPSLocation) obj;
		return Double.doubleToLongBits(latitude) == Double.doubleToLongBits(other.latitude)
				&& Double.doubleToLongBits(longitude) == Double.doubleToLongBits(other.longitude);
	}
	
	@Override
	public String toString() {
		return "GeoCoordinate [latitude=" + latitude + ", longitude=" + longitude + "]";
	}

	@Override
	public int compareTo(GPSLocation that) {
		int result = Double.compare(this.latitude, that.latitude);

		return (0 == result) ? Double.compare(this.longitude, that.longitude) : result;
	}

	/**
	 * Compare GPS locations.
	 */
	private static final class GPSLocationComparator implements Comparator<GPSLocation> {

		@Override
		public int compare(GPSLocation o1, GPSLocation o2) {

			return o1.compareTo(o2);
		}
	}
}
