import request from '@/utils/request'
export function getList(params) {
    
    return request({  
      url: '/ProResource/Load',
      method: 'get',
      params
    })
  }
  export function add(data) {
    return request({
      url: '/ProResource/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/ProResource/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/ProResource/DeleteEntity',
      method: 'post',
      data
    })
  }

  export function getlistbystepId(params) {
    return request({
      url: '/ProResource/GetDesoutterByStepId/',
      method: 'get',
      params
    })
  }
  