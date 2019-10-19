import requests as r
import json
import pandas as pd
import sys
import utm

base_url = 'https://api.globe.gov/search/v1/measurement/protocol/'

query_params = {
  'protocols': 'mosquito_habitat_mapper',
  'geojson': "TRUE",
  'sample': "FALSE"
}

res = r.get(base_url, params = query_params)

data = res.json()

#print(json.dumps(data['features'], indent=4))

parsedData = []

for obs in data['features']:
  parsedObs = obs['properties']
  
  latitude = obs['geometry']['coordinates'][1]
  longitude = obs['geometry']['coordinates'][0]
  
  del parsedObs['mosquitohabitatmapperMeasurementLatitude']
  del parsedObs['mosquitohabitatmapperMeasurementLongitude']  

  parsedObs['latitude'] = latitude
  parsedObs['longitude'] = longitude

  parsedData.append(parsedObs)

df = pd.DataFrame(parsedData)
df.to_csv('data.csv')

with open('parsedData.json', 'w') as ff:
  json.dump(parsedData, ff)

