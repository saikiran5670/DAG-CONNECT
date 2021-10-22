package net.atos.daf.ct2;

import java.util.TreeSet;

public class TreeSetDemo1 {

    public static void main(String[] args) {

        Node node1 = new Node(1.1,1.9);
        Node node2 = new Node(2.3,2.4);
        Node node3 = new Node(3.3,3.4);
        Node node4 = new Node(4.5,4.6);
        Node node5 = new Node(5.7,5.8);


        TreeSet<Node> nodeTree = new TreeSet<>(new DistanceComparator(node3));
        nodeTree.add(node3);
        nodeTree.add(node1);
        nodeTree.add(node5);
        nodeTree.add(node4);
        nodeTree.add(node2);


        System.out.println("-----------------------------------");
        nodeTree.stream()
                .forEach(System.out::println);
        System.out.println("========================");
        System.out.println("Test 4.2");
        Node testNode = new Node(4.2,4.2);
        Node floor = nodeTree.floor(testNode);
        System.out.println("floor "+floor);
        System.out.println("========================");
        System.out.println("Test 2.2");
        Node testNode2 = new Node(2.2,2.2);
        Node floor2 = nodeTree.floor(testNode2);
        System.out.println("floor "+floor2);
        System.out.println("========================");
        System.out.println("Test 5.2");
        Node testNode3 = new Node(5.2,5.2);
        Node floor3 = nodeTree.floor(testNode3);
        System.out.println("floor "+floor3);


    }
}
