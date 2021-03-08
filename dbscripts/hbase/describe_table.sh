#!/bin/bash

if [ "$#" -ne 1 ]; then
  printf "Usage \n"
  printf "   $0 <tablename> \n" >&2
  printf "Or \n" >&2
  printf "   $0 <namespace:tablename>\n" >&2
  exit 1
fi

echo "describe '$1' " | hbase shell -n
