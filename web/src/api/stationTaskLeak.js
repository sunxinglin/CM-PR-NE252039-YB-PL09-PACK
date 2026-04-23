import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/Base_StationTaskLeak/Update',
      method: 'post',
      data
    })
  }
  export function add(data) {
    return request({
      url: '/Base_StationTaskLeak/Add',
      method: 'post',
      data
    })
  }
  export function del(data) {
    return request({
      url: '/Base_StationTaskLeak/DeleteEntity',
      method: 'post',
      data
    })
  }
  
export function GetByTaskId(params) {
    return request({
      url: '/Base_StationTaskLeak/GetByTaskId',
      method: 'get',
      params
    })
  }