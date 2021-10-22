package net.atos.daf.ct2;

public class Node implements Comparable<Node>{

    double lat;
    double lon;

    public Node(double lat, double lon) {
        this.lat = lat;
        this.lon = lon;
    }

    public double getLat() {
        return lat;
    }

    public void setLat(double lat) {
        this.lat = lat;
    }

    public double getLon() {
        return lon;
    }

    public void setLon(double lon) {
        this.lon = lon;
    }

    @Override
    public int compareTo(Node o) {
        return Double.valueOf(this.lat).compareTo(Double.valueOf(o.lat));
    }

    @Override
    public String toString() {
        return "Node{" +
                "lat:" + lat +
                ", lon:" + lon +
                '}';
    }
}
