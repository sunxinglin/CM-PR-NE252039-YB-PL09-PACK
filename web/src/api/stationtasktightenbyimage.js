import request from '@/utils/request'
export function add(data) {
    return request({
      url: '/StationTaskTightenByImage/Add',
      method: 'post',
      data
    })
  }

  export function update(data) {
    return request({
      url: '/StationTaskTightenByImage/Update',
      method: 'post',
      data
    })
  }

    
  export function del(data) {
    return request({
      url:  '/StationTaskTightenByImage/DeleteEntity',
      method: 'post',
      data
    })
  }

  export function Load(params) {
    const p = Object.assign({}, params)
    if (p.taskId === undefined && p.taskid !== undefined) {
      p.taskId = p.taskid
      delete p.taskid
    }
    return request({
      url: '/StationTaskTightenByImage/LoadByTaskId',
      method: 'get',
      params: p
    })
}

export function UploadImage(data) {
  return request({
    url: '/StationTaskTightenByImage/UploadImage',
    method: 'post',
    data
  })
}

export function GetImageUrl(params) {
  return request({
    url: '/StationTaskTightenByImage/GetImageUrl',
    method: 'get',
    params
  })
}

export function LoadTaskList(params) {
  return request({
    url: '/StationTaskTightenByImage/LoadTaskList',
    method: 'get',
    params
  })
}

export function LoadLayout(params) {
  return request({
    url: '/StationTaskTightenByImage/LoadLayout',
    method: 'get',
    params
  })
}

export function SaveLayout(data) {
  return request({
    url: '/StationTaskTightenByImage/SaveLayout',
    method: 'post',
    data
  })
}
