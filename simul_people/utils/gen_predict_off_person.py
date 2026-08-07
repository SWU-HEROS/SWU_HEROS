import sys 

from copy import deepcopy
from datetime import datetime
from bson import ObjectId
from dataGenerator.utils.make_new_doc import * 


# Repeat to add person documents based on the predicted number of people
def create_next_document_predict(prev_doc, iter_num, dy, dz, x, y, z, flag):
    
    new_doc = deepcopy(prev_doc)
    new_doc["_id"] = ObjectId()
    new_doc["datetime"] = (datetime.datetime.fromisoformat(prev_doc["datetime"]) + datetime.timedelta(minutes=5)).isoformat()
    
    # TODO: Refactor this into a separate function (also used for simulation updates)
    
    # Update the coordinates of people from the previous document
    for cell in new_doc["cells"]:
        for person in cell["people"]:
            old_loc = person["location"]
            if "movement_direction" in person:
                person["movement_direction"] = [
                    person["movement_direction"][0],  # x
                    person["movement_direction"][1] + dy,  # y
                    person["movement_direction"][2] + dz   # z
                ]
    
    
    for _ in range(iter_num):
        # Call the data generator function
        add_person_to_cell(cell, flag, x, y, z)

    return new_doc


# Update people's coordinates during the simulation
def update_people_coord(prev_doc, y, z):
    
    new_doc = deepcopy(prev_doc)
    new_doc["_id"] = ObjectId()
    
    
    new_doc["datetime"] = (datetime.datetime.fromisoformat(prev_doc["datetime"]) + datetime.timedelta(seconds=10)).isoformat()
    
    for cell in new_doc["cells"]:
        for person in cell["people"]:
            old_loc = person["location"]
            if "movement_direction" in person:
                person["movement_direction"] = [
                    person["movement_direction"][0],  # x
                    person["movement_direction"][1] + y,  # y
                    person["movement_direction"][2] + z   # z
                ]
                
    return new_doc






