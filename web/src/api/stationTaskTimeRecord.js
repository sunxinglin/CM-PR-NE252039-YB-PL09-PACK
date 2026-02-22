import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/Base_StationTaskRecordTime/UpdateConfig',
      method: 'post',
      data
    })
  }
  export function add(data) {
    return request({
      url: '/Base_StationTaskRecordTime/AddConfig',
      method: 'post',
      data
    })
  }
  export function del(data) {
    return request({
      url: '/Base_StationTaskRecordTime/DeleteConfig',
      method: 'post',
      data
    })
  }
  
export function load(params) {
    return request({
      url: '/Base_StationTaskRecordTime/LoadConfig',
      method: 'get',
      params
    })
  }