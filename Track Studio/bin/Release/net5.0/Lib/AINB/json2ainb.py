import json
import ainb

with open("in", 'r', encoding='utf-8') as file:
    data = json.load(file)
file = ainb.AINB(data, from_dict=True)
with open("out", 'wb') as outfile:
    file.ToBytes(file, outfile)