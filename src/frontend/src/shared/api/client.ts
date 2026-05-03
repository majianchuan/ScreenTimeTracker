import axios, { type AxiosInstance } from "axios";

export const baseUrl = import.meta.env.DEV ? "http://127.0.0.1:52069" : "";

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
