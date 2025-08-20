import sys 

from copy import deepcopy
from datetime import datetime
from bson import ObjectId
from dataGenerator.utils.make_new_doc import * 


#예측 사람 수 만큼 반복해서 사람 doc 추가 
def create_next_document_predict(prev_doc, iter_num, dy, dz, x, y, z, flag):
    
    new_doc = deepcopy(prev_doc)
    new_doc["_id"] = ObjectId()
    new_doc["datetime"] = (datetime.datetime.fromisoformat(prev_doc["datetime"]) + datetime.timedelta(minutes=5)).isoformat()
    
    #함수로 나중에 빼기...시뮬레이션 업데이트에도 똑같이 있음 
    
    #이전 문서의 사람들 좌표 업데이트
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
        #data generator 함수 호출
        add_person_to_cell(cell, flag, x, y, z)

    return new_doc


#시뮬레이션 씬 동안 사람들 좌표 업데이트하는 함수 
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






