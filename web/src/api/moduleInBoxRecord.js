import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_ModuleInBoxRecord/Load',
        method: 'post',
        data
    })

}

export function modelExpornt(data) {
    return request({
        url: '/Proc_ModuleInBoxRecord/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

