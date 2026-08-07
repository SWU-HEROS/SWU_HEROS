from fastapi import APIRouter
from crud.db_crud import read_data
from api.api_v1.endpoints.second_preprocessing import second_preprocess
import time

router = APIRouter()

update_data = []
age = [0, 0, 0, 0]

@router.get('/third')
async def second_preprocess():
    
    # 1. Record the start time
    start_time = time.time()
    print(f"[level 3] Start time: {start_time}", flush=True)
    
    # 2. Read data from the database
    data = await read_data()
    
    
    #print(data, flush=True)
    
    # 3. Iterate through person objects
    for privacy in data:
        privacy = privacy.dict()
        
        for cell in privacy['cells']:
            # 4. Remove the existing age distribution
            del cell['age_distribution']
            
            # 5. Initialize a new age distribution
            cell['age_distribution'] ={
                "youth": 0,
                "middle_aged": 0,
                "senior": 0, 
                "elderly": 0
            }

            # 6. Apply level 2 pseudonymization
            for person in cell['people']:
                
                # 7. Remove mobile number, IMSI, and gender information
                del person['mobile_number']
                del person['IMSI']
                del person['gender']
                
                # 8. Apply age pseudonymization
                if person['age'] > 20 and person['age'] <30 :
                    person['age'] = 'mid_20s'
                    age[0]+=1
                
                elif person['age'] > 30 and person['age'] < 40 :
                    person['age'] = 'mid_30s'
                    age[0]+=1

                elif person['age'] > 40 and person['age'] < 50 :
                    person['age'] = 'mid_40s'
                    age[1]+=1

                elif person['age'] > 50 and person['age'] < 60 :
                    person['age'] = 'mid_50s'
                    age[2]+=1

                elif person['age'] > 60 and person['age'] < 70 :
                    person['age'] = 'mid_60s'
                    age[3]+=1
                
                elif person['age'] > 70:
                    person['age'] = 'mid_70s'
                    age[4]+=1
                
            # 9. Update the age distribution
            cell['age_distribution']['youth'] = age[0]
            cell['age_distribution']['middle_aged'] = age[1]
            cell['age_distribution']['senior'] = age[2]
            cell['age_distribution']['elderly'] = age[3]
        
        update_data.append(privacy) 
        
    # 10. Record the end time
    end_time = time.time()
    print(f"[level 3] End time: {end_time}", flush=True)

    # 11. Calculate the execution time
    execution_time = end_time - start_time
    print(f"[level 3] Execution time: {execution_time} seconds", flush=True)
                     
    return update_data
