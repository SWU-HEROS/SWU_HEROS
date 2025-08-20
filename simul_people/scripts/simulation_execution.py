import sys
import time
import random
from multiprocessing import Process

from simul_people.db.mongo import *
from simul_people.crud.mongo_crud import * 
from simul_people.utils.gen_predict_off_person import *
from simul_people.utils.predict_people import *

'''
이전 DB에서 이미 문서를 가져왔던 상태이면(중단했다가 다시 실행했을 때) -> 에러 발생
추후에 에러 핸들링 추가 필요 
'''

def insert_main_latest_doc(main_collection, simul_collection):
    collection = connect_mongo(main_collection)
    new_collection = connect_mongo(simul_collection)
    
    main_doc = get_latest_document(collection)
    create_main_doc(new_collection, main_doc)


#하차
#아래 -> 위
def gen_prediction_get_off_num():
    
    collection = connect_mongo('test_collection_B')
    
    payload = make_model_payload()
    
    get_off_num = int(request_get_off_num(payload) // 10)
    

    #이전 문서 가져오기 -> 시뮬레이션 시작 시, 메인 씬의 가장 최근 도큐먼트를 새 컬렉션에 가져오기 
    insert_main_latest_doc('people_data', 'test_collection_B')
    
    for _ in range(10):
    
        
        prev_doc = get_latest_document(collection)
        
        #새 문서 만들기
        #create_next_document_predict(prev_doc, iter_num, dy, dz, x, y, z, flag) -> 이전문서, 반복, y 이동, z이동, x, y, z시작, flag
        new_doc = create_next_document_predict(prev_doc, get_off_num, 1.11, -6.03, round(random.uniform(-17, 17), 2), 2, 496, 0)
        
        #새 문서 db에 넣기
        result = create_document(collection, new_doc)
        
        print('test_collection_B', result)
        
        #사람 추가 되기 전까지 계속 사람들 좌표 업데이트 
        for _ in range(2):
            
            time.sleep(30)
            
            prev_coor = get_latest_document(collection)
            #update_people_coord(prev_doc, y, z)
            #이전 문서, y이동, z이동
            coor_doc = update_people_coord(prev_coor, 1.11, -6.03)
            
            coor_up = create_document(collection, coor_doc)
            
            print(coor_up)

#승차
#위 -> 아래 
def gen_prediction_get_on_num():
    
    collection = connect_mongo('test_collection_A')
    
    payload = make_model_payload()
    
    get_on_num = int(request_get_on_num(payload) // 10)
    
    #이전 문서 가져오기 -> 시뮬레이션 시작 시, 메인 씬의 가장 최근 도큐먼트를 새 컬렉션에 가져오기 
    insert_main_latest_doc('test_collection', 'test_collection_A')
    
    for _ in range(10):
    
        
        prev_doc = get_latest_document(collection)
        
        #새 문서 만들기
        #create_next_document_predict(prev_doc, iter_num, dy, dz, x, y, z, flag) -> 이전문서, 반복, y 이동, z이동, x, y, z시작, flag
        new_doc = create_next_document_predict(prev_doc, get_on_num, -1.11, 6.03, round(random.uniform(-17, 17), 2), 68, 110, 1)
        
        #새 문서 db에 넣기
        result = create_document(collection, new_doc)
        
        print('test_collection_A', result)
        
        #사람 추가 되기 전까지 계속 사람들 좌표 업데이트 
        for _ in range(2):
            
            time.sleep(30)
            
            prev_coor = get_latest_document(collection)
            
            #update_people_coord(prev_doc, y, z)
            #이전 문서, y이동, z이동
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
    