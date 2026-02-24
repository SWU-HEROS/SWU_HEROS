# PrivCrowd Twin

**3D 디지털 트윈 기반 인파 관리 시스템**은 실세계 데이터를 활용하여 인파 혼잡도를 시뮬레이션하고, AI를 통한 미래 인파 예측 및 개인정보 비식별화 기능을 제공하는 분석 도구입니다.

## 주요 기능 (Main Features)

* **지하철 승객 데이터 학습**: 통신 소스의 근실시간 데이터와 서울시 열린데이터 광장의 지하철 승객 데이터를 수집하며, 2022년부터 2024년까지의 데이터를 활용해 교차 검증으로 Prophet 모델을 학습시킵니다.
* **머신러닝 기반 군중 데이터 생성**: 학습된 모델을 통해 현재 시간, 월, 요일, 할로윈 여부 등의 요소를 기반으로 3D 시뮬레이션에서 미래의 승객 수를 예측합니다.
* **선택적 개인정보 비식별화**: 데이터를 개인·가명·익명 정보로 분류하고, 3D 디지털 트윈에서 API 호출이 발생하는 시점에 실시간으로 비식별화를 수행하여 프라이버시를 보호합니다.
* **3D 시뮬레이션 기반 군중 밀도 시각화**: 실제와 동일한 3D 환경을 $1m^2$ 단위 셀로 나누어 시각화하며, 8명 이상 밀집 시 '매우 위험'으로 분류하여 위험 지역을 직관적으로 식별하게 합니다.

## 설치 방법 (Installation)

### 1. 저장소 클론 (Clone the repository)
```bash
git clone [https://github.com/SWU-HEROS/SWU_HEROS.git](https://github.com/SWU-HEROS/SWU_HEROS.git)
cd SWU_HEROS
```

### 2. 의존성 설치 (Install dependencies)
* **Unity**: Unity 2022.3.28f1 LTS 버전이 필요합니다.
* **Python**: 다음 명령어를 통해 백엔드 의존성을 설치합니다.
```bash
cd dataGenerator
pip install requirements.txt

cd ..
cd predictAPI
pip install requirements.txt
```

## 사용 방법 (Usage)
1. **데이터베이스 설정**: MongoDB를 실행하고 수집된 지하철 승하차 데이터를 임포트합니다.
2. **백엔드 서버 실행**: FastAPI를 통해 비식별화 및 예측 API를 활성화합니다.
```bash
uvicorn main:app --reload
```
3. **시뮬레이션 실행**: Unity 프로젝트를 열고 Monitoring 또는 Simulation 모드를 선택하여 실행합니다.
4. **결과 확인**:
   * 디지털 트윈 화면에서 셀별 색상 변화를 확인합니다.
   * 시뮬레이션 종료 후 총 소요 시간 및 무정차 통과 횟수 리포트를 확인합니다.

## 프로젝트 구조 (Project Structure)
```text
SWU-HEROS/
├── dataGenerator/          # MongoDB에 저장되는 각 객체 정보 생성
├── predictAPI/             # 지하철 승하차 인원수 예측
├── simul_people/           # 시뮬레이션 시 각 객체 좌표 업데이트
└── unitySystem/            # 3D 디지털 트윈 시스템
```
## 데이터 비식별화 모델 (De-identification Model)
* **Level 1 (Identifiable Personal Information Level)**: 모든 정보 포함 (전화번호, IMSI 등).
* **Level 2 (Partially Masked Personal Information Level)**: 전화번호 일부 마스킹(010-****-5432), IMSI 삭제, 나이 그룹화.
* **Level 3 (Fully Masked Personal Information Level)**: 전화번호/IMSI 완전 삭제, 나이대 광범위 일반화(Youth, Senior 등).

## 라이선스 (License)
이 프로젝트는 MIT 라이선스 하에 배포됩니다.
