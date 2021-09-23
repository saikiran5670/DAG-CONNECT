package org.geofence;

import static java.lang.Math.max;
import static java.lang.Math.min;

public class RayCasting {

    static boolean intersects(double[] A, double[] B, double[] P) {
        if (A[1] > B[1])
            return intersects(B, A, P);

        if (P[1] == A[1] || P[1] == B[1])
            P[1] += 0.0001;

        if (P[1] > B[1] || P[1] < A[1] || P[0] >= max(A[0], B[0]))
            return false;

        if (P[0] < min(A[0], B[0]))
            return true;

        double red = (P[1] - A[1]) / (double) (P[0] - A[0]);
        double blue = (B[1] - A[1]) / (double) (B[0] - A[0]);
        return red >= blue;
    }

    static boolean isInside(double[][] polygonPoints, double[] pnt) {
        boolean inside = false;
        int len = polygonPoints.length;
        for (int i = 0; i < len; i++) {
            if (intersects(polygonPoints[i], polygonPoints[(i + 1) % len], pnt))
                inside = !inside;
        }
        return inside;
    }

    public static void main(String[] args) {
        //TODO TEST Square inside
        double [][] squareBoundaryPoint = {{0,0},{10,0},{10,10},{0,10}};
        double [] pnt = {4,6};  //inside point
        double [] outPnt = {12,12};  //Outside point
        double [] boundaryPnt = {10,2};  //Boundary point
        double [] lambdaBoundaryPnt = {9.00009,1.9999};  //Boundary point
        System.out.println("------------------Square Test ---------");
        System.out.println("Square Test Inside : "+isInside(squareBoundaryPoint,pnt));
        System.out.println("Square Test Inside : "+isInside(squareBoundaryPoint,outPnt));
        System.out.println("Square Test Inside : "+isInside(squareBoundaryPoint,boundaryPnt));
        System.out.println("Square Test Inside : "+isInside(squareBoundaryPoint,lambdaBoundaryPnt));

        //TODO Polygon test
        double [][] polygonBoundaryPoint = {{0.0,0},{5.0,2.0},{8.0,0.0},{6.0,8.0},{10,10},{12,12},{0,8}};
        double [] pnt2 = {2,6};  //inside point
        double [] outPnt2 = {6.0,12};  //Outside point
        double [] boundaryPnt2 = {6,8};  //Vertex point
        double [] lambdaOutBoundaryPnt2 = {8.0,6.0};  //Slightly out Boundary point
        double [] lambdaInBoundaryPnt2 = {11,11};  //Slightly In Boundary point
        System.out.println("------------------Polygon Test ---------");
        System.out.println("polygon Test Inside : "+isInside(polygonBoundaryPoint,pnt2));
        System.out.println("polygon Test Inside : "+isInside(polygonBoundaryPoint,outPnt2));
        System.out.println("polygon Test Inside : "+isInside(polygonBoundaryPoint,boundaryPnt2));
        System.out.println("polygon Test Inside : "+isInside(polygonBoundaryPoint,lambdaOutBoundaryPnt2));
        System.out.println("polygon Test Inside : "+isInside(polygonBoundaryPoint,lambdaInBoundaryPnt2));

        //TODO Hexagon Test
        double [][] hexagonBoundaryPoint = {{6,2},{10,4},{10,8},{6.0,10.0},{2,8},{2,4}};
        double [] pnt3 = {4,6};  // inside point
        double [] outPnt3 = {-4,6};  // Outside point
        double [] boundaryPnt3 = {10,8};  // Vertex point
        double [] lambdaOutBoundaryPnt3 = {10.0,6.0};  // Slightly out Boundary point
        double [] lambdaInBoundaryPnt3 = {9.009,5.009};  // Slightly In Boundary point
        System.out.println("------------------Hexagon Test ---------");
        System.out.println("hexagon Test Inside : "+isInside(hexagonBoundaryPoint,pnt3));
        System.out.println("hexagon Test Inside : "+isInside(hexagonBoundaryPoint,outPnt3));
        System.out.println("hexagon Test Inside : "+isInside(hexagonBoundaryPoint,boundaryPnt3));
        System.out.println("hexagon Test Inside : "+isInside(hexagonBoundaryPoint,lambdaOutBoundaryPnt3));
        System.out.println("hexagon Test Inside : "+isInside(hexagonBoundaryPoint,lambdaInBoundaryPnt3));

    }
}
