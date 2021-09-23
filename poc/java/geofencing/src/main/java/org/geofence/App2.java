package org.geofence;

public class App2 {

    public static void main(String[] args) {
        double[][] polygonBoundaryPointSqr = {{18.588016803584118, 73.67856645052323},
                {18.573535293949636, 73.67925309603105},
                {18.574226854685545, 73.69989537660966},
                {18.587640024343475, 73.69809150695801}};

        double[] insidePointSqr = {18.58144739481477,73.69356536333451}; // inside test
        double[] outSidePointSqr = {18.582098245376045, 73.70146178667436}; // Outside test
        double[] boundaryPointSqr = {18.582810278177107,73.69873805126909}; // Boundary test
        double[] vertexPointSqr = {18.587640024343475, 73.69809150695801}; // Vertex test
        double[] lambdaOutSqr = {18.582474685068934, 73.69918727342973}; // Near out test
        double[] lambdaInSqr = {18.58597295652171,73.6979212707747}; // Near In test

        System.out.println("----------- Ray casting Square -------------------");
        System.out.println("polygon Test Inside : "+RayCasting.isInside(polygonBoundaryPointSqr,insidePointSqr));
        System.out.println("polygon Test Outside : "+RayCasting.isInside(polygonBoundaryPointSqr,outSidePointSqr));
        System.out.println("Point in Boundary Test : "+RayCasting.isInside(polygonBoundaryPointSqr,boundaryPointSqr));
        System.out.println("Point on vertx Test : "+RayCasting.isInside(polygonBoundaryPointSqr,vertexPointSqr));
        System.out.println("Point Near out Test : "+RayCasting.isInside(polygonBoundaryPointSqr,lambdaOutSqr));
        System.out.println("Point Near In Test : "+RayCasting.isInside(polygonBoundaryPointSqr,lambdaInSqr));

        System.out.println("----------- Ray casting Custom polygon-------------------");

        double[][] polygonBoundaryPoint = {{18.586776635254807, 73.68251715419363},
                {18.57973937489426, 73.68277464625906},
                {18.581641090957326, 73.6865082812078},
                {18.580125821587423, 73.69045649287771},
                {18.585271580879706, 73.6906710695989},
                {18.584213967149587, 73.6864009928472}};

        double[] insidePoint = {18.582902110229224,73.68627224681448}; // inside test
        double[] outSidePoint = {18.583044482786615, 73.69236622569632}; // Outside test
        double[] boundaryPoint = {18.583415667823427,73.69055305240225}; // Boundary test
        double[] vertexPoint = {18.584213967149587, 73.6864009928472}; // Vertex test
        double[] lambdaOut = {18.581391936852146, 73.6865082812078}; // Near out test
        double[] lambdaIn = {18.584348711571437,73.68725393531393}; // Near In test

        System.out.println("polygon Test Inside : "+RayCasting.isInside(polygonBoundaryPoint,insidePoint));
        System.out.println("polygon Test Outside : "+RayCasting.isInside(polygonBoundaryPoint,outSidePoint));
        System.out.println("Point in Boundary Test : "+RayCasting.isInside(polygonBoundaryPoint,boundaryPoint));
        System.out.println("Point on vertx Test : "+RayCasting.isInside(polygonBoundaryPoint,vertexPoint));
        System.out.println("Point Near out Test : "+RayCasting.isInside(polygonBoundaryPoint,lambdaOut));
        System.out.println("Point Near In Test : "+RayCasting.isInside(polygonBoundaryPoint,lambdaIn));


    }
}
