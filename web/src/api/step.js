import request from '@/utils/request'

export function load(params) {
  return request({
    url: '/Step/Load',
    method: 'get',
    params
  })
}

export function GetStepsByProductId(params) {
  return request({
    url: '/Step/GetStepsByProductId',
    method: 'get',
    params
  })
}

export function getList(params) {
  return request({
    url: '/Step/GetList',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Step/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Step/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Step/DeleteEntity',
    method: 'post',
    data
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
