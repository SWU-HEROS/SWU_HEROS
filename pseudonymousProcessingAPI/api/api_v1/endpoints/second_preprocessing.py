from fastapi import APIRouter
from crud.db_crud import read_data
import time

router = APIRouter()

update_data = []

@router.get('/second')
async def second_preprocess():
    # 1. Record the start time
    start_time = time.time()
    print(f"[level 2] Start time: {start_time}", flush=True)

    # 2. Read data from the database
    data = await read_data()
    
    # 3. Extract person objects
    for privacy in data:
        privacy = privacy.dict()
        
        for cell in privacy['cells']:
            for person in cell['people']:
                
                # 4. Remove IMSI and mobile number information
                del person['mobile_number']
                del person['IMSI']
                
                # 5. Apply age pseudonymization
                if person['age'] > 20 and person['age'] <30 :
                    person['age'] = 'mid_20s'
                
                elif person['age'] > 30 and person['age'] < 40 :
                    person['age'] = 'mid_30s'

                elif person['age'] > 40 and person['age'] < 50 :
                    person['age'] = 'mid_40s'

                elif person['age'] > 50 and person['age'] < 60 :
                    person['age'] = 'mid_50s'

                elif person['age'] > 60 and person['age'] < 70 :
                    person['age'] = 'mid_60s'
                
                elif person['age'] > 70:
                    person['age'] = 'mid_70s'   
                                  
        update_data.append(privacy)

    # 6. Record the end time
    end_time = time.time()
    print(f"[level 2] End time: {end_time}", flush=True)

    # 7. Calculate the execution time
    execution_time = end_time - start_time
    print(f"[level 2] Execution time: {execution_time} seconds", flush=True)           

    return  update_data
