import sys
import time
import random
from multiprocessing import Process

from simul_people.db.mongo import *
from simul_people.crud.mongo_crud import * 
from simul_people.utils.gen_predict_off_person import *
from simul_people.utils.predict_people import *

# TODO: Add error handling for restart scenarios.

def insert_main_latest_doc(main_collection, simul_collection):
    collection = connect_mongo(main_collection)
    new_collection = connect_mongo(simul_collection)
    
    main_doc = get_latest_document(collection)
    create_main_doc(new_collection, main_doc)


# Alighting
# Move from bottom to top
def gen_prediction_get_off_num():
    
    collection = connect_mongo('test_collection_B')
    
    payload = make_model_payload()
    
    get_off_num = int(request_get_off_num(payload) // 10)
    

    # Retrieve the latest document from the main scene and copy it to the
    # simulation collection when the simulation starts.
    insert_main_latest_doc('people_data', 'test_collection_B')
    
    for _ in range(10):
    
        
        prev_doc = get_latest_document(collection)
        
        # Create a new document
        # create_next_document_predict(prev_doc, iter_num, dy, dz, x, y, z, flag)
        # -> previous document, iteration count, y movement, z movement,
        #    starting x, y, z coordinates, and flag
        new_doc = create_next_document_predict(prev_doc, get_off_num, 1.11, -6.03, round(random.uniform(-17, 17), 2), 2, 496, 0)
        
        # Insert the new document into the database
        result = create_document(collection, new_doc)
        
        print('test_collection_B', result)
        
        # Continue updating people's coordinates until new people are added
        for _ in range(2):
            
            time.sleep(30)
            
            prev_coor = get_latest_document(collection)
            # update_people_coord(prev_doc, y, z)
            # previous document, y movement, z movement
            coor_doc = update_people_coord(prev_coor, 1.11, -6.03)
            
            coor_up = create_document(collection, coor_doc)
            
            print(coor_up)

# Boarding
# Move from top to bottom
def gen_prediction_get_on_num():
    
    collection = connect_mongo('test_collection_A')
    
    payload = make_model_payload()
    
    get_on_num = int(request_get_on_num(payload) // 10)
    
    # Retrieve the latest document from the main scene and copy it to the
    # simulation collection when the simulation starts.
    insert_main_latest_doc('test_collection', 'test_collection_A')
    
    for _ in range(10):
    
        
        prev_doc = get_latest_document(collection)
        
        # Create a new document
        
        # create_next_document_predict(prev_doc, iter_num, dy, dz, x, y, z, flag)
        # -> previous document, iteration count, y movement, z movement,
        #    starting x, y, z coordinates, and flag
        new_doc = create_next_document_predict(prev_doc, get_on_num, -1.11, 6.03, round(random.uniform(-17, 17), 2), 68, 110, 1)
        
        # Insert the new document into the database
        result = create_document(collection, new_doc)
        
        print('test_collection_A', result)
        
        # Continue updating people's coordinates until new people are added
        for _ in range(2):
            
            time.sleep(30)
            
            prev_coor = get_latest_document(collection)
            
            # update_people_coord(prev_doc, y, z)
            # previous document, y movement, z movement
            coor_doc = update_people_coord(prev_coor, -1.11, 6.03)
            
            coor_up = create_document(collection, coor_doc)
            
            print(coor_up)

            
if __name__=="__main__":
    
    get_on = Process(target=gen_prediction_get_on_num, daemon=False)
    get_off = Process(target=gen_prediction_get_off_num, daemon=False)
    
    
    get_on.start()
    get_off.start()
    
    get_on.join()
    get_off.join()
    
    print("done")
    
