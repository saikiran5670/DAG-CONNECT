#!/bin/bash

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <namespace>" >&2
  exit 1
fi

echo "drop_namespace '$1'" | hbase shell -n
