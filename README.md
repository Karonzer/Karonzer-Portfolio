Unity Roguelite Action Game

Unity 6 / C# / Event-Driven Architecture

Unity 기반으로 제작한 Roguelite 액션 게임 포트폴리오입니다.
확장 가능한 구조 설계와 실제 문제 해결 경험을 중심으로 구성했습니다.

핵심 요약

장르: Roguelite / Action (Vampire-Survivors 스타일)

엔진 / 언어: Unity 6 / C#

설계 방향

Manager 중심 구조

이벤트(Action) 기반 통신

ScriptableObject를 활용한 데이터 분리

Object Pooling & Addressables 기반 최적화

주요 시스템

Game / Spawn / Skill / Stat / Upgrade / UI Manager

시스템 간 직접 참조 최소화

기능 추가 시 기존 코드 수정 없이 확장 가능

공격 & 업그레이드

AttackRoot 추상 클래스 기반 공격 시스템

각 공격은 로직만 분리된 자식 클래스 구조

레벨업 시 랜덤 업그레이드 카드 제공

중복 레벨업은 큐 방식으로 처리

스폰 & 문제 해결 사례

NavMesh 기반 랜덤 스폰 시스템

일반 몬스터 / 보스 / 웨이브 분리 관리

문제

몬스터가 정상 스폰 후 추적 시 (0,0,0)에서 이동 시작

해결

NavMeshAgent 초기화 순서 문제 분석

Warp() 포함 초기화 로직 분리로 해결

UI & 성능

UIManager 기반 이벤트 연동 UI

16:9 비율 대응

투사체 / 몬스터 / 이펙트 풀링

Update 최소화, 이벤트 중심 처리
