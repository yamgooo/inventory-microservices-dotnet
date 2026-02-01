export interface ApiResponse<T = any> {
  success: boolean;
  message: string | null;
  errors: string[] | null;
  timestamp: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}