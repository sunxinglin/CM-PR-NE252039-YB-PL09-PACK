import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/Base_StationTaskPressure/Update',
      method: 'post',
      data
    })
  }
  export function add(data) {
    return request({
      url: '/Base_StationTaskPressure/Add',
      method: 'post',
      data
    })
  }
  export function del(data) {
    return request({
      url: '/Base_StationTaskPressure/DeleteEntity',
      method: 'post',
      data
    })
  }
  
export function GetByTaskId(params) {
    return request({
      url: '/Base_StationTaskPressure/GetByTaskId',
      method: 'get',
      params
    })
  }