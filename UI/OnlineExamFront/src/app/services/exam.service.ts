import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { exam } from '../models/exam';

@Injectable({
  providedIn: 'root'
})
export class ExamService {
  url = "https://localhost:44302/api/Exams";
  selectedExam: exam | undefined;
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  constructor(private http: HttpClient) { }
  //Get All exams
  getAllExam(): Observable<exam[]> {
    var result = this.http.get<exam[]>(this.url);
    return result;
  }
  updateExam(exam: exam): Observable<exam> {
    return this.http.put<exam>(this.url, exam, this.httpOptions);
  }
  addExam(exam: exam): Observable<exam> {
    console.log(exam);
    return this.http.post<exam>(this.url, exam, this.httpOptions);
  }
  getExam(id: number): Observable<exam> {
    return this.http.get<exam>(this.url + "/" + id);
  }

  getExamWithQuestions():Observable<exam>{
    return this.http.get<exam>(this.url+'/GetExamWithQuestions/');
  }
    delete (id:any){
    const ans = confirm('Do you want to delete exa, with id: ' + id);
    if ( ans ) {
    return this .http.delete(`${this.url}/${id}`);
    }

  
  }
}
