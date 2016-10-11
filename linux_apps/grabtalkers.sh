#! /bin/sh

while true; do
echo 'going in'
psql --username=postgres --dbname=trim_mm --command="select count(*) from nameidmap where sampleday = -2;"

python ./posters/posters.py > ~/talkers

for i in `cat ~/talkers`; do psql --username=postgres --dbname=trim_mm --command="insert into nameidmap (name, sampleday, postsperyear) VALUES('$i', -2, 1);" > nul 2> nul ;  done

rm ~/talkers

echo 'after the latest feed parse'
psql --username=postgres --dbname=trim_mm --command="select count(*) from nameidmap where sampleday = -2;"
echo '2nd in line'
psql --username=postgres --dbname=trim_mm --command="select count(*) from nameidmap where sampleday = -1;"
echo 'already scraped'
psql --username=postgres --dbname=trim_mm --command="select count(*) from nameidmap where sampleday > 0;"

sleep 60
echo 'nap is over'
done


