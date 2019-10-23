# Mosquito Defenders

> Mosquitos are being sighted all over the globe. As head of Mosquito Command, can you respond in time, put your resources in the right place and prevent a global outbreak of disease?  

Mosquito Defenders is our entry for the "1UP for NASA Earth!" challenge in the [2019 NASA Spaceapps Challenge held in Exeter](https://2019.spaceappschallenge.org/locations/exeter-devon/).

In the game, we imagine what it might be like if citizen scientists were the last line of defense against mosquito-borne diseases like malaria. You play as the head of a public health agency tasked with responding to mosquito sighting reports from these citizen scientists. You have a limited budget of money and people available, and you'll need to spread them over the globe to stall or prevent disease outbreaks.

## How to run
### Requirements
This game was made using Unity, with a python web server serving mosquito outbreak events. You will need the following installed:
 - Unity
 - Python3
 - pipenv

To set up the server, run the following commands.
```
cd Data
pipenv --python3.7 install
```

### Running
To run the server, run the following from the `Data/` directory.
```
pipenv make serve
```
__If the address from the output of the command doesn't match http://127.0.0.1:5000, you will need to update the `Assets/ngrok.txt` file to match the address of the server.__
With the python server running, open the project in Unity and build for your platform.

## The data

This game draws its data from the [NASA Globe citizen science programme](www.globe.gov), specifically the mosquito habitat dataset. We use the Globe API to randomly provide real mosquito observations. Each observation can come with a lat/long, mosquito counts, information about what spources of water they are breeding in, species ID and associated photos taken with the Globe app. This means that the alerts you see in the game are based on real mosquito sightings from all over the world. 

We also use information about the global prevalence of malaria to model disease spread. In the game, mosquito sightings begin as just reports of mosquito activity, but if left unchecked they will spread. Reports in malaria-prone areas can also generate disease outbreaks. The idea is that as part of the game you will have to decide how to allocate your resources appropriately. There may be a lot of mosquito reports from the US - a wealthy country with plenty of citizen scientists - but will these actually lead to malarial infections?

## Other interesting features

Our team's split of skills meant that we weren't all cut out to work on the game directly. Two of us were data scientists who weren't really familiar with Unity/C#. So instead we built a simple disease modelling and event server in Flask. This server responds to periodic requests from the game proper with information about mosquito sightings and disease spread. It's janky but we had fun making it.
