#!/bin/bash

if [ "$#" -lt 2 ]; then
  printf "Usage: \n" >&2
  printf " $0 <tablename> <column_family # 1> <column_family # n \n" >&2
  exit 1
fi

if [ "$#" -gt 8 ]; then
  printf "Usage: \n" >&2
  printf " $0 <tablename> <column_family # 1> <column_family # n \n" >&2
  printf " Please restrict column family upto 6 \n" >&2
  exit 1
fi

for ((i = 1; i <= $#; i++ )); do
  printf '%s\n' "Arg $i: ${!i}"
  if [ "$i" -eq 1  ]; then
  str1+="create '${!i}'"
  elif [ "$i" -gt 1 ]; then
  str1+=" , '${!i}'"
  fi
done

printf "Creating table in progress... \n"
printf "$str1 \n"

echo $str1 | hbase shell -n 
