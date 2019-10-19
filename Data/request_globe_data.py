import requests
import json
import numpy as np
import pandas as pd
import geopandas as gpd
import sys
import geopandas as gpd
import numpy as np

base_url = 'https://api.globe.gov/search/v1/measurement/protocol/'

query_params = {
  'protocols': 'mosquito_habitat_mapper',
  'geojson': "TRUE",
  'sample': "FALSE"
}

res = requests.get(base_url, params = query_params)

data = res.json()

#print(json.dumps(data['features'], indent=4))

parsedData = []

for obs in data['features']:
  parsedObs = obs['properties']
  latitude = obs['geometry']['coordinates'][1]
  longitude = obs['geometry']['coordinates'][0]

  parsedObs['latitude'] = latitude
  parsedObs['longitude'] = longitude

  parsedData.append(parsedObs)

# pass through geodataframe format and keep only key columns
df = pd.DataFrame(parsedData)
gdf = gpd.GeoDataFrame(
    parsedData,
    geometry = gpd.points_from_xy(df.longitude, df.latitude),
    crs = {'init' :'epsg:4326'}
).rename(columns = {
    "countryName": "country",
    "mosquitohabitatmapperMeasuredAt": "timestamp",
    "mosquitohabitatmapperWaterSource": "waterSource",
    "mosquitohabitatmapperWaterSourceType": "sourceType",
    "mosquitohabitatmapperLarvaeCount": "larvaeCount",
    "mosquitohabitatmapperMosquitoAdults": "adults",
    "mosquitohabitatmapperMosquitoEggs": "eggs",
    "mosquitohabitatmapperMosquitoPupae": "pupae",
    "mosquitohabitatmapperGenus": "genus",
    "mosquitohabitatmapperSpecies": "species",
    "mosquitohabitatmapperLarvaFullBodyPhotoUrls": "larvae_images",
    "mosquitohabitatmapperAbdomenCloseupPhotoUrls": "abdomen_images",
    "mosquitohabitatmapperWaterSourcePhotoUrls": "water_images",
}).drop(columns = {
    "mosquitohabitatmapperMeasurementLongitude",
    "mosquitohabitatmapperMeasurementLatitude",
    "countryCode",
    "mosquitohabitatmapperBreedingGroundEliminated",
    "elevation",
    "mosquitohabitatmapperDataSource",
    "mosquitohabitatmapperMeasurementElevation",
    "mosquitohabitatmapperMosquitoHabitatMapperId",
    "mosquitohabitatmapperUserid",
    "organizationId",
    "organizationName",
    "protocol",
    "siteId",
    "siteName",
    "mosquitohabitatmapperLastIdentifyStage",
    "mosquitohabitatmapperComments",
    "geometry"
})

# convert strings to bools and create new column for if there were signs of mosquitos

dct = {'true': True, 'false': False}

gdf["adults"] = gdf["adults"].map(dct)
gdf["eggs"] = gdf["eggs"].map(dct)
gdf["pupae"] = gdf["pupae"].map(dct)
gdf['seen'] = gdf[["adults", "eggs", "pupae"]].any(axis=1)

def coalesce(df, column_names):
    i = iter(column_names)
    column_name = next(i)
    answer = df[column_name]
    for column_name in i:
        answer = answer.fillna(df[column_name])
    return answer

def first_image_only(series):
    newvar = []
    for (i, photos) in enumerate(series):
        if np.isreal(photos) and np.isnan(photos):
            newvar.append("") # no image
        else:
            newvar.append(photos.split(";")[0]) # first listed image
    return newvar

gdf['image_url'] = coalesce(gdf, ["abdomen_images", "larvae_images", "water_images"])
gdf['image_url'] = first_image_only(gdf.image_url)

gdf = gdf[gdf.seen]

def generate_text(row):
    text = "Mosquito activity detected!\n"
    
    types = []
    if row['adults']:
        types.append('adults')
    if row['eggs']:
        types.append('eggs')
    if row['pupae']:
        types.append('pupae')
    
    if len(types) == 1:
        text += types[0].capitalize() + " found"
    if len(types) == 2:
        text += types[0].capitalize() + " and " + types[1] +" found"  
    if len(types) == 3:
        text += types[0].capitalize() + ", " + types[1] + " and " + types[2] +" found"
    if (row["waterSource"] not in ["other", "discarded: other"]):
        text += " in " + row["waterSource"]
    text += "."
    return(text)
    
gdf['text'] = gdf.apply(generate_text, axis=1)

gdf = gdf[[
  'latitude',
  'longitude',
  'text',
  'image_url',
]]


with open('processed/mosquito_data.json', 'w') as ff:
    ff.write(json.dumps(list(gdf.T.to_dict().values()), indent=4, sort_keys=True))
