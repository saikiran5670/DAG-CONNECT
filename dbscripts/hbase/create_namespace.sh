#!/bin/bash

if [ "$#" -ne 1 ]; then
  printf "Usage: \n">&2
  printf "$0 <namespace> \n">&2
  exit 1
fi

echo "create_namespace '$1'" | hbase shell -n
