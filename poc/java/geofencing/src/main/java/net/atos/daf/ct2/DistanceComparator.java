package net.atos.daf.ct2;

import java.util.Comparator;

public class DistanceComparator implements Comparator<Node> {

    private Node rootNode;

    public DistanceComparator(Node rootNode) {
        this.rootNode = rootNode;
    }

    @Override
    public int compare(Node o1, Node o2) {
        double x2o1 = Math.pow((o1.getLat() - rootNode.getLat()), 2.0);
        double y2o1 = Math.pow((o1.getLat() - rootNode.getLon()), 2.0);
        Double disto1 = Math.sqrt(x2o1 + y2o1);
        double x2o2 = Math.pow((o2.getLat() - rootNode.getLat()), 2.0);
        double y2o2 = Math.pow((o2.getLat() - rootNode.getLon()), 2.0);
        Double disto2 = Math.sqrt(x2o2 + y2o2);
        /*System.out.println("node 1 :"+o1);
        System.out.println("node 2 :"+o2);
        System.out.println("node 1 distance :"+disto1);
        System.out.println("node 2 distance :"+disto2);
        System.out.println("***********************************");*/
        return disto1.compareTo(disto2);
    }
}
