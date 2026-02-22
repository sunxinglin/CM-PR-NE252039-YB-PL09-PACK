import request from '@/utils/request'

export function load(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/Load',
    method: 'get',
    params
  })
}

export function changstatus(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/ChangStatusById',
    method: 'get',
    params
  })
}

export function ClearBomDataById(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/ClearBomDataById',
    method: 'get',
    params
  })
}

export function ClearDataById(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/ClearDataById',
    method: 'get',
    params
  })
}

export function loadStationTaskDetail(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/LoadStationTaskDetail',
    method: 'get',
    params
  })
}

export function DeleteScan(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/DeleteScan',
    method: 'get',
    params
  })
}

export function ChangeHasUp(params) {
  return request({
    url: '/Proc_StationTask_Main_StatusQuery/ChangeHasUp',
    method: 'get',
    params
  })
}