import json
import ainb

with open("in", 'rb') as file:
    data = file.read()
file = ainb.AINB(data)
with open("out", 'w', encoding='utf-8') as outfile:
    json.dump(file.output_dict, outfile, ensure_ascii=False, indent=2)