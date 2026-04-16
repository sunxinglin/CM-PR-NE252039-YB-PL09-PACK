import request from "@/utils/request";

export function loadExternalAutoTightenData(params) {
  return request({
    url: "/AutoTighten/LoadExternalAutoTightenData",
    method: "get",
    params,
  });
}

export function loadExternalAutoTightenDataDetail(params) {
  return request({
    url: "/AutoTighten/LoadExternalAutoTightenDataDetail",
    method: "get",
    params,
  });
}

export function uploadExternalData(data) {
  return request({
    url: "/AutoTighten/UploadExternalData",
    method: "post",
    data,
  });
}

export function exportExternalAutoTightenDataDetail(params) {
  return request({
    url: "/AutoTighten/ExportExternalAutoTightenDataDetail",
    method: "get",
    params,
    responseType: "blob",
  });
}
