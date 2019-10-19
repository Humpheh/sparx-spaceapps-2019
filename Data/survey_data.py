
## Map.ox explorer

#https://map.ox.ac.uk/explorer/#/

#Plasmodium vivax PR Surveys
#Village level survey clusters measuring P. vivax prevalence

#AND

#Plasmodium falciparum PR Surveys
#Village level survey clusters measuring P. falciparum prevalence


import pandas as pd
import json

pf_data = pd.read_csv('raw_data/public_pf_data.csv')
pf_data['prop_pf_pos'] = 100 * pf_data['pf_pos'] / pf_data['examined']
pf_data = pf_data[pf_data.prop_pf_pos > 0][["latitude", "longitude"]].dropna()

pv_data = pd.read_csv('raw_data/public_pv_data.csv')
pv_data['prop_pv_pos'] = 100 * pv_data['pv_pos'] / pv_data['examined']
pv_data = pv_data[pv_data.prop_pv_pos > 0][["latitude", "longitude"]].dropna()

#pf_data.append(pv_data).plot.scatter("longitude", "latitude")


data = pv_data.append(pf_data)
with open('processed/survey_data.json', 'w') as ff:
  ff.write(json.dumps(list(data.T.to_dict().values())))

