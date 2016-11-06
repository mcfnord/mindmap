import sys
import xml.etree.ElementTree as ET
import json
username = sys.argv[1]
tree = ET.parse('./xml_foafs/' + username)
root = tree.getroot()
data = {}

print username

if root.find('iRead') is not None:
    data = []
    for iReadEm in root.find('iRead').findall('u'):
        data.insert(0, iReadEm.text)
    data = {'iRead': data}

data['name'] = root.find('name').text ;

if root.find('repliesGot') is not None:
    data['repliesGot'] = int(root.find('repliesGot').text)

if root.find('city') is not None:
    data['city'] = root.find('city').text

if root.find('country') is not None:
    data['country'] = root.find('country').text

if root.find('posts') is not None:
    data['posts'] = int(root.find('posts').text)

if root.find('replies') is not None:
    data['replies'] = int(root.find('replies').text)

with open('./json/' + username.replace(".foaf.xml", ".json"), 'w') as outfile:
    json.dump(data, outfile)
