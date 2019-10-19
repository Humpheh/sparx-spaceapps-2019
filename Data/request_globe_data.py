import requests as r
import json
import pandas as pd
import sys

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
  #parsedObs = obs['properties']
  parsedObs = {}  
  latitude = obs['geometry']['coordinates'][1]
  longitude = obs['geometry']['coordinates'][0]

  parsedObs['latitude'] = latitude
  parsedObs['longitude'] = longitude

  parsedData.append(parsedObs)

# pass through geodataframe format and keep only key columns

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

df["adults"] = df["adults"].map(dct)
df["eggs"] = df["eggs"].map(dct)
df["pupae"] = df["pupae"].map(dct)
df['seen'] = df[["adults", "eggs", "pupae"]].any(axis=1)

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

gdf['image_name'] = coalesce(gdf, ["abdomen_images", "larvae_images", "water_images"])
gdf['image_name'] = first_image_only(gdf.image_name)

gdf = gdf.drop(
    columns = {"abdomen_images", "larvae_images", "water_images"}
)

with open('processed/mosquito_data.json', 'w') as ff:
    json.dump(parsedData, ff)
