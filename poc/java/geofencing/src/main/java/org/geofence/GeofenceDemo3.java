package org.geofence;


import java.util.Arrays;
import java.util.List;
import java.util.Objects;
import java.util.stream.Collectors;

public class GeofenceDemo3 {

    public static void main(String[] args) {

        // Test Data Square
        double[][] polygonBoundaryPoint = {{18.588016803584118, 73.67856645052323},
                {18.573535293949636, 73.67925309603105},
                {18.574226854685545, 73.69989537660966},
                {18.587640024343475, 73.69809150695801}};

        System.out.println("-------- Square point in inside Test -------------");
        double[] insidePoint = {18.580844808000027,73.69362670787332}; // inside test
        System.out.println(isInside(insidePoint,polygonBoundaryPoint));

        System.out.println("-------- Square point on Outside Test -------------");
        double[] outSidePoint = {18.581658373689724, 73.70139438518045}; // Outside test
        System.out.println(isInside(outSidePoint,polygonBoundaryPoint));

        System.out.println("-------- Square point on Boundary Test -------------");
        double[] boundaryPoint = {18.582370148979088,73.69879724964878}; // Boundary test
        System.out.println(isInside(boundaryPoint,polygonBoundaryPoint));

        System.out.println("-------- Square point on vertx Test -------------");
        double[] vertexPoint = {18.588016803584118, 73.67856645052323}; // Vertex test
        System.out.println(isInside(vertexPoint,polygonBoundaryPoint));

        System.out.println("-------- Square Slightly out Test -------------");
        double[] lambdaOut = {18.582098245376045, 73.69931601946244}; // Slightly out test
        System.out.println(isInside(lambdaOut,polygonBoundaryPoint));

        System.out.println("-------- Square Slightly In Test -------------");
        double[] lambdaIn = {18.582220279579563,73.69811438982377}; // Slightly In test
        System.out.println(isInside(lambdaIn,polygonBoundaryPoint));

    }

    public static double findMax(double[] pointArray){
        return Arrays.stream(pointArray).max().getAsDouble();
    }

    public static double findMin(double[] pointArray){
        return Arrays.stream(pointArray).min().getAsDouble();
    }

    public static Boolean isInside(double [] point, double[][] polygonBoundaryPoint){
        double[] latArr = new double[polygonBoundaryPoint.length];
        double[] lonArr = new double[polygonBoundaryPoint.length];
        for(int i=0; i< polygonBoundaryPoint.length; i++){
            latArr[i] = polygonBoundaryPoint[i][0];
            lonArr[i] = polygonBoundaryPoint[i][1];
        }
        double latMax = findMax(latArr);
        double lonMax = findMax(lonArr);
        double latMin = findMin(latArr);
        double lonMin = findMin(lonArr);

        //3.	Check if point under consideration say  is inside / outside Bounding box as follows
        if(point[0] < latMin || point[1] < lonMin || point[0] > latMax || point[1] > lonMax )
            return Boolean.FALSE;
        else{
//            Calculate latitude of center(near) of Bounding Box φ0   as follows ( MAX(lat) + MIN(lat) )/2
            double φ0 = (latMax + latMin) / 2;
            /**
             * Convert all vertices of Polygon to (X,Y) system using following function
             */
            Point aXY =  convertLatLongToXY(point, φ0);
            return isInConvexPolygon(aXY, latLonToXY(polygonBoundaryPoint, φ0));

        }
    }

    /**
     * φ0 – Take it from step 4
     * r  - Radius of earth sphere - 6,378 km as per  https://en.wikipedia.org/wiki/Earth_radius
     * ConvertLatLongToXY( lng, lat)
     * {
     *      Var X = r   * lng * cos(φ0)
     *      Var Y = r   * lat
     * }
     */
    public static Point convertLatLongToXY(double [] point, double φ0){
        int r = 6378;   // Radius of earth sphere
        double x = r * point[0] * Math.cos(φ0);
        double y = r * point[1];
        return new Point(x,y);
    }
   //Convert all vertices of Polygon to (X,Y)
    public static List<Point> latLonToXY(double[][] polyBoundaryPoints, double φ0 ){
        List<Point> polygonPointsXY = Arrays.stream(polyBoundaryPoints)
                .map(points -> convertLatLongToXY(points, φ0))
                .collect(Collectors.toList());
        return polygonPointsXY;
    }

    public static Boolean isInConvexPolygon(Point a, List<Point> polygonPointXY){
        //Declare direction counters
        int pos=0;
        int neg=0;
        //Loop through vertices of Polygon
        for(int i=0; i < polygonPointXY.size(); i++){
            //If point A  is lying on current vertex of polygon
            if (polygonPointXY.get(i).equals(a))
                return Boolean.TRUE;
            //Form a segment between the i'th point
            double x1 = polygonPointXY.get(i).x;
            double y1 = polygonPointXY.get(i).y;
            //And the i+1'th, or if i is the last, with the first point
            int i2 = (i+1) % polygonPointXY.size();
            double x2 = polygonPointXY.get(i2).x;
            double y2 = polygonPointXY.get(i2).y;
            double x = a.x;
            double y = a.y;
            //Compute the cross product
            double d = (x - x1)*(y2 - y1) - (y - y1)*(x2 - x1);
            if (d > 0) pos++;
            if (d < 0) neg++;
            //If the sign changes, then point is outside //if only one is nozero point is inside // both are non zero point is outside
            if (pos > 0 && neg > 0)
                return Boolean.FALSE;

        }
        //If no change in direction, then on same side of all segments, and thus inside
        return Boolean.TRUE;
    }


     static class Point{
          double x;
          double y;
       public Point(double x, double y){
           this.x = x;
           this.y = y;
       }

         @Override
         public boolean equals(Object o) {
             if (this == o) return true;
             if (o == null || getClass() != o.getClass()) return false;
             Point point = (Point) o;
             return Double.compare(point.x, x) == 0 && Double.compare(point.y, y) == 0;
         }

         @Override
         public int hashCode() {
             return Objects.hash(x, y);
         }
     }
}
