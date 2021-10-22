package org.geofence;

import net.atos.daf.ct2.DistanceComparator;
import net.atos.daf.ct2.Node;
import org.junit.Before;
import org.junit.Test;

import java.util.TreeSet;

public class NearestTest {

    private TreeSet<Node> nodeTree;

    @Before
    public void initTree(){
        Node node1 = new Node(2.0,1.0);
        Node node2 = new Node(2,4);
        Node node3 = new Node(3,6);
        Node node4 = new Node(5.7,4);
        Node node5 = new Node(7,7.1);
        Node node6 = new Node(8.1,5.4);
        Node node7 = new Node(8.2,3.5);
        Node node8 = new Node(7.5,2.3);
        Node node9 = new Node(6.5,1);

        nodeTree = new TreeSet<>(new DistanceComparator(node5));
        nodeTree.add(node1);
        nodeTree.add(node2);
        nodeTree.add(node3);
        nodeTree.add(node4);
        nodeTree.add(node5);
        nodeTree.add(node6);
        nodeTree.add(node7);
        nodeTree.add(node8);
        nodeTree.add(node9);
    }

    @Test
    public void eclipseTest(){

        nodeTree.stream()
                .forEach(System.out::println);
        System.out.println("-----------------------------------");

        Node testNode1 = new Node(1.9,2.1);
        System.out.println("========================");
        System.out.println("Test Node: "+testNode1);
        Node floor = nodeTree.floor(testNode1);
        System.out.println("floor "+floor);

        Node testNode2 = new Node(2.5,5.1);
        System.out.println("========================");
        System.out.println("Test Node: "+testNode2);
        floor = nodeTree.floor(testNode1);
        System.out.println("floor "+floor);

        Node testNode3 = new Node(6.1,7);
        System.out.println("========================");
        System.out.println("Test Node: "+testNode3);
        floor = nodeTree.floor(testNode3);
        System.out.println("floor "+floor);

        Node testNode4 = new Node(8,4.5);
        System.out.println("========================");
        System.out.println("Test Node: "+testNode4);
        floor = nodeTree.floor(testNode4);
        System.out.println("floor "+floor);
    }
}
