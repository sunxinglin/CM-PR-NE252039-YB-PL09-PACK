import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/Base_StationTaskModuleInBox/Update',
      method: 'post',
      data
    })
  }
  export function add(data) {
    return request({
      url: '/Base_StationTaskModuleInBox/Add',
      method: 'post',
      data
    })
  }
  export function del(data) {
    return request({
      url: '/Base_StationTaskModuleInBox/DeleteEntity',
      method: 'post',
      data
    })
  }
  
export function GetByTaskId(params) {
    return request({
      url: '/Base_StationTaskModuleInBox/GetByTaskId',
      method: 'get',
      params
    })
  }