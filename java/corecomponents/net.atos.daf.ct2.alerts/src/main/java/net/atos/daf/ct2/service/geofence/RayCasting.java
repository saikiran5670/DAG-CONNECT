package net.atos.daf.ct2.service.geofence;

import static java.lang.Math.max;
import static java.lang.Math.min;

public class RayCasting {

    public static Boolean intersects(Double[] A, Double[] B, Double[] P) {
        if (A[1] > B[1])
            return intersects(B, A, P);

        if (P[1] == A[1] || P[1] == B[1])
            P[1] += 0.0001;

        if (P[1] > B[1] || P[1] < A[1] || P[0] >= max(A[0], B[0]))
            return Boolean.FALSE;

        if (P[0] < min(A[0], B[0]))
            return Boolean.TRUE;

        double red = (P[1] - A[1]) / (double) (P[0] - A[0]);
        double blue = (B[1] - A[1]) / (double) (B[0] - A[0]);
        return red >= blue;
    }

    public  static Boolean isInside(Double[][] polygonPoints, Double[] pnt) {
        Boolean inside = Boolean.FALSE;
        int len = polygonPoints.length;
        for (int i = 0; i < len; i++) {
            if (intersects(polygonPoints[i], polygonPoints[(i + 1) % len], pnt))
                inside = !inside;
        }
        return inside;
    }

}
