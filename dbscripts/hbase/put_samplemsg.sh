#!/bin/bash

echo "put '$1', 'r1', '$2:column1', 'data1'" | hbase shell -n
echo "put '$1', 'r2', '$2:column2', 'data2'" | hbase shell -n
