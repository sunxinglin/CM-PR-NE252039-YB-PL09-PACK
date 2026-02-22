import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/Base_StationTaskGlueShouJian/Update',
      method: 'post',
      data
    })
  }
  export function add(data) {
    return request({
      url: '/Base_StationTaskGlueShouJian/Add',
      method: 'post',
      data
    })
  }
  export function del(data) {
    return request({
      url: '/Base_StationTaskGlueShouJian/DeleteEntity',
      method: 'post',
      data
    })
  }
  
export function GetByTaskId(params) {
    return request({
      url: '/Base_StationTaskGlueShouJian/GetByStationCode',
      method: 'get',
      params
    })
  }
export function GetGuleShouJianData(params) {
    return request({
      url: '/Proc_GlueShouJian/GetGuleShouJianData',
      method: 'get',
      params
    })
  }
  export function UpLoadGlueDataCommAgain_ShouJian(params) {
    return request({
      url: '/Proc_GlueShouJian/UpLoadGlueDataCommAgain_ShouJian',
      method: 'post',
      params
    })
  }