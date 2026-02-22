import request from '@/utils/request'

export function load(params) {
  return request({
    url: '/Station/Load',
    method: 'get',
    params
  })
}

export function GetAllStation() {
  return request({
    url: '/Station/GetAllStation',
    method: 'get'
  })
}

export function GetStationsByFlowId(params) {
  return request({
    url: '/Station/GetStationsByFlowId',
    method: 'get',
    params
  })
}
export function GetStationsByStepId(params) {
  return request({
    url: '/Station/GetStationsByStepId',
    method: 'get',
    params
  })
}

export function getList(params) {
  return request({
    url: '/Station/GetList',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Station/Add',
    method: 'post',
    data
  })
}

export function addBom(data) {
  return request({
    url: '/Station/AddBom',
    method: 'post',
    data
  })
}
export function update(data) {
  return request({
    url: '/Station/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Station/DeleteEntity',
    method: 'post',
    data
  })
}


export function getstationcode(params) {

  return request({
    url: '/Station/GetStationCodeById',
    method: 'get',
    params
  })
}
  //导入工位BOm文件
  export function imporntStationFile(data) {
    return request({
      url: '/Station/ImporntStationFile',
      method: 'post',
      data
    })
  }
//导出工位BOm文件
export function ModelExpornt(data) {
  return request({
    url: '/Station/ModelExpornt',
    method: 'post',
    data,
    responseType: "blob"
  })
}
  

