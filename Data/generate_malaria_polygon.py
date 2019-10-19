import pandas as pd
import json
import geopandas as gpd
import alphashape
import matplotlib.pyplot as plt
from descartes import PolygonPatch
import geopandas

pf_data = pd.read_csv('raw/public_pf_data.csv')
pf_data['prop_pf_pos'] = 100 * pf_data['pf_pos'] / pf_data['examined']
pf_data = pf_data[pf_data.prop_pf_pos > 0][["latitude", "longitude"]].dropna()
pv_data = pd.read_csv('raw/public_pv_data.csv')
pv_data['prop_pv_pos'] = 100 * pv_data['pv_pos'] / pv_data['examined']
pv_data = pv_data[pv_data.prop_pv_pos > 0][["latitude", "longitude"]].dropna()

data = pv_data.append(pf_data)

gdf = gpd.GeoDataFrame(
    data, 
    geometry = gpd.points_from_xy(data.longitude, data.latitude),
    crs = {'init' :'epsg:4326'}
)

points = gdf[['longitude', 'latitude']].values

alpha_shape = alphashape.alphashape(points, 0.01)

json = geopandas.GeoSeries([alpha_shape]).to_json()
with open('processed/malaria_polygon.json', 'w') as ff:
    ff.write(json)

# Cool plot!   
#world = gpd.read_file(gpd.datasets.get_path('naturalearth_lowres'))
#ax = world.plot(figsize = (25, 15))
#ax.add_patch(PolygonPatch(alpha_shape, alpha=0.2))
#gdf.plot(ax = ax, color = "red", markersize = 3) 
