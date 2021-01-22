#!/bin/bash

if [ "$#" -ne 1 ]; then
  printf "Usage \n"
  printf "   $0 <tablename>\n" >&2
  printf "Or>\n" >&2
  printf "   $0 <namespace:tablename>\n" >&2
  exit 1
fi

printf "Disable table in progress...\n"
echo "disable '$1' " | hbase shell -n

printf "Drop table in progress...\n"
echo "drop '$1' " | hbase shell -n
