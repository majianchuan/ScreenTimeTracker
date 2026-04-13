import axios, { type AxiosInstance } from "axios";
import { kebabCase } from "case-anything";

export const baseUrl = import.meta.env.DEV ? "http://localhost:5124" : "";

export const baseApiUrl = `${baseUrl}/api`;

export const apiClient: AxiosInstance = axios.create({
  baseURL: baseApiUrl,
  timeout: 2000,
  paramsSerializer: {
    indexes: null,
  },
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const method = config.method?.toLowerCase();
  if ((method === "get" || method === "delete") && config.params) {
    config.params = convertParamsToKebab(config.params);
  }
  return config;
});

function convertParamsToKebab(
  params: Record<string, unknown>,
): Record<string, unknown> {
  if (!params || typeof params !== "object") return params;

  const newParams: Record<string, unknown> = {};
  Object.keys(params).forEach((key) => {
    newParams[kebabCase(key)] = params[key];
  });

  return newParams;
}
