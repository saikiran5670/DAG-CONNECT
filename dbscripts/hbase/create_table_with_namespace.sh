#!/bin/bash

if [ "$#" -lt 3 ]; then
  printf "Usage: \n" >&2
  printf " $0 <namespace> <tablename> <column_family # 1> <column_family # n \n" >&2
  exit 1
fi

if [ "$#" -gt 9 ]; then
  printf "Usage: \n" >&2
  printf " $0 <namespace> <tablename> <column_family # 1> <column_family # n \n" >&2
  printf " Please restrict column family upto 6 \n" >&2
  exit 1
fi

for ((i = 1; i <= $#; i++ )); do
  printf '%s\n' "Arg $i: ${!i}"
  if [ "$i" -eq 1  ]; then
  str2=${!i}
  elif [ "$i" -eq 2  ]; then
  str1+="create '$str2:${!i}'"
  elif [ "$i" -gt 2  ]; then
  str1+=" , '${!i}'"
  fi
done

printf "Creating table in progress... \n"
printf "$str1 \n"

echo $str1 | hbase shell -n 
