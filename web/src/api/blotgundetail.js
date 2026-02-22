import request from '@/utils/request'
export function getList(data) {
    return request({
      url: '/StationTask_BlotGunDetail/GetBlotGunDetail',
      method: 'post',
      data
    })
  }
  export function getAutoBlotList(params) {
    return request({
      url: '/AutoTighten/LoadAutoTightenData',
      method: 'get',
      params
    })
  }

  export function getAutoBlotDetailList(params) {
    return request({
      url: '/AutoTighten/LoadAutoTightenDataDetail',
      method: 'get',
      params
    })
  }


//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/StationTask_BlotGunDetail/BlotGunModelExpornt',
        method: 'post',
        data,
    })
}

//����Pack BOm�ļ�
export function automodelExpornt(data) {
    return request({
        url: '/StationTask_BlotGunDetail/AutoBlotGunModelExpornt',
        method: 'post',
        data,
    })
}