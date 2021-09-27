package org.geofence;

import static org.geofence.GeofenceDemo3.isInside;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args ) {
        // Test Data custome
        double[][] polygonBoundaryPoint = {{18.586776635254807, 73.68251715419363},
                {18.57973937489426, 73.68277464625906},
                {18.581641090957326, 73.6865082812078},
                {18.580125821587423, 73.69045649287771},
                {18.585271580879706, 73.6906710695989},
                {18.584213967149587, 73.6864009928472}};
        
        double[] insidePoint = {18.583410583102335,73.68631516215872}; // inside test
        double[] outSidePoint = {18.587763048815585, 73.68687306163382}; // Outside test
        double[] boundaryPoint = {18.583389529111702,73.69059343812678}; // Boundary test
        double[] vertexPoint = {18.580125821587423, 73.69045649287771}; // Vertex test
        double[] lambdaOut = {18.581163142479735, 73.68655119655203}; // Slightly out test
        double[] lambdaIn = {18.584213988092216,73.68704472301077}; // Slightly In test
        double[] lambdaIn2 = {18.584264814113737,73.68708763835501}; // Slightly In test
        double[] lambdaIn3 = {18.583843458114927,73.68730765941754}; // Slightly In test
        double[] lambdaIn4 = {18.584357012872253,73.68727010849133}; // Slightly In test

        System.out.println("-----------------------");
        System.out.println("Point in inside Test : "+isInside(insidePoint,polygonBoundaryPoint));
        System.out.println("Point in Outside Test : "+isInside(outSidePoint,polygonBoundaryPoint));
        System.out.println("Point in Boundary Test : "+isInside(boundaryPoint,polygonBoundaryPoint));
        System.out.println("Point on vertx Test : "+isInside(vertexPoint,polygonBoundaryPoint));
        System.out.println("Point Slightly out Test : "+isInside(lambdaOut,polygonBoundaryPoint));
        System.out.println("Point Slightly In Test : "+isInside(lambdaIn,polygonBoundaryPoint));
        System.out.println("Point Slightly In Test2 : "+isInside(lambdaIn2,polygonBoundaryPoint));
        System.out.println("Point Slightly In Test3 : "+isInside(lambdaIn3,polygonBoundaryPoint));
        System.out.println("Point Slightly In Test4 : "+isInside(lambdaIn3,polygonBoundaryPoint));

        System.out.println("----------- Ray casting -------------------");
        System.out.println("polygon Test Inside : "+RayCasting.isInside(polygonBoundaryPoint,insidePoint));
        System.out.println("polygon Test Outside : "+RayCasting.isInside(polygonBoundaryPoint,outSidePoint));
        System.out.println("Point in Boundary Test : "+RayCasting.isInside(polygonBoundaryPoint,boundaryPoint));
        System.out.println("Point on vertx Test : "+RayCasting.isInside(polygonBoundaryPoint,vertexPoint));
        System.out.println("Point Slightly out Test : "+RayCasting.isInside(polygonBoundaryPoint,lambdaOut));
        System.out.println("Point Slightly In Test : "+RayCasting.isInside(polygonBoundaryPoint,lambdaIn));
        System.out.println("Point Slightly In Test2 : "+RayCasting.isInside(polygonBoundaryPoint,lambdaIn2));
        System.out.println("Point Slightly In Test3 : "+RayCasting.isInside(polygonBoundaryPoint,lambdaIn3));
        System.out.println("Point Slightly In Test4 : "+RayCasting.isInside(polygonBoundaryPoint,lambdaIn4));


        // Test Data Square
        double[][] polygonBoundaryPointSquare = {{18.588016803584118, 73.67856645052323},
                {18.573535293949636, 73.67925309603105},
                {18.574226854685545, 73.69989537660966},
                {18.587640024343475, 73.69809150695801}};


        double[] insidePointSqr = {18.580844808000027,73.69362670787332}; // inside test
        double[] outSidePointsqr = {18.581658373689724, 73.70139438518045}; // Outside test
        double[] boundaryPointSqr = {18.582370148979088,73.69879724964878}; // Boundary test
        double[] vertexPointSqr = {18.588016803584118, 73.67856645052323}; // Vertex test
        double[] lambdaOutSqr = {18.582098245376045, 73.69931601946244}; // Slightly out test
        double[] lambdaInSqr = {18.582220279579563,73.69811438982377}; // Slightly In test

       /* System.out.println("-------- Square point in inside Test -------------");
        System.out.println("Point in inside Test : "+isInside(insidePointSqr,polygonBoundaryPointSquare));
        System.out.println("Point in Outside Test : "+isInside(outSidePointsqr,polygonBoundaryPointSquare));
        System.out.println("Point in Boundary Test : "+isInside(boundaryPointSqr,polygonBoundaryPointSquare));
        System.out.println("Point on vertx Test : "+isInside(vertexPointSqr,polygonBoundaryPointSquare));
        System.out.println("Point Slightly out Test : "+isInside(lambdaOutSqr,polygonBoundaryPointSquare));
        System.out.println("Point Slightly In Test : "+isInside(lambdaInSqr,polygonBoundaryPointSquare));

        System.out.println("----------- Ray casting -------------------");
        System.out.println("polygon Test Inside : "+RayCasting.isInside(polygonBoundaryPointSquare,insidePointSqr));
        System.out.println("polygon Test Outside : "+RayCasting.isInside(polygonBoundaryPointSquare,outSidePointsqr));
        System.out.println("Point in Boundary Test : "+RayCasting.isInside(polygonBoundaryPointSquare,boundaryPointSqr));
        System.out.println("Point on vertx Test : "+RayCasting.isInside(polygonBoundaryPointSquare,vertexPointSqr));
        System.out.println("Point Slightly out Test : "+RayCasting.isInside(polygonBoundaryPointSquare,lambdaOutSqr));
        System.out.println("Point Slightly In Test : "+RayCasting.isInside(polygonBoundaryPointSquare,lambdaInSqr));*/
    }
}
