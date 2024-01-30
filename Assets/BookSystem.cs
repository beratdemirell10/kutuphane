using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.UI;


public class BookSystem : MonoBehaviour
{
   
        [SerializeField] Text txt_allBook;
        [SerializeField] InputField input_author;
        [SerializeField] InputField input_book;
        [SerializeField] InputField input_search;
        public Button borrowButton;
        string[] bookInfoArray;
        string myFilePath, fileName;
        public Button searchButton;
        [SerializeField] Text borrow_Book;
        public Button returnButton;
        public Button dueButton;
        void Start()
        {
            fileName = "Library.txt";
            myFilePath = "Assets/" + fileName;
        
        }


    // Tüm kitaplarý listelediðim fonksiyon burada library.txt dosyasýndan kitap adý, yazar adý ve kitap sayýsýný inceleyip tek tek text'e yazdýrýyor
        void DisplayAllBooks()
        {
            txt_allBook.text = "";
            System.Array.Sort(bookInfoArray);
            foreach (string bookInfo in bookInfoArray)
            {
                string[] bookData = bookInfo.Split(',');

                if (bookData.Length >= 3)
                {
                    string bookTitle = bookData[0].Trim();
                    string bookAuthor = bookData[1].Trim();
                    string bookCount = bookData[2].Trim();
                    txt_allBook.text += $"<color=red>  {bookTitle}  --->   {bookAuthor}  --->   {bookCount}</color>\n";
                }
            }
        }

        public void ReadFromTheFile()
        {
            bookInfoArray = File.ReadAllLines(myFilePath);
            DisplayAllBooks();
        }

    // kitap ekleme fonksiyonuyla inputfield'den aldýðým textleri listeye ekliyoruz.
        public void AddToTheFile()
        {
            string author = input_author.text; 
            string book = input_book.text;
            bool found = false; 
            List<string> newBookInfoList = new List<string>(); 
            foreach (string bookInfo in bookInfoArray)
            {
                string[] bookData = bookInfo.Split(',');
                if (bookData.Length >= 3)
                {
                    string bookTitle = bookData[0].Trim();
                    string bookAuthor = bookData[1].Trim();
                    int bookCount = int.Parse(bookData[2].Trim());
                    if (bookTitle.Trim().ToLower() == book.Trim().ToLower() && bookAuthor.Trim().ToLower() == author.Trim().ToLower()) 
                    {
                        bookCount++; 
                        found = true; 
                    }
                    newBookInfoList.Add($"{bookTitle}, {bookAuthor}, {bookCount}");
                }
            }
            if (!found) 
            {
                newBookInfoList.Add($"{book}, {author}, 1"); 
            }
            File.WriteAllLines(myFilePath, newBookInfoList.ToArray()); 
            ReadFromTheFile(); 
        }
    //küçük büyük harf duyarlýlýðý olmadan kitabý aradýðýmýz fonksiyon.
        public void SearchBook()
        {
            string search = input_search.text; 
            txt_allBook.text = ""; 
            foreach (string bookInfo in bookInfoArray)
            {
                string[] bookData = bookInfo.Split(','); 
                if (bookData.Length >= 3) 
                {
                    string bookTitle = bookData[0].Trim().ToLower(); 
                    string bookAuthor = bookData[1].Trim().ToLower(); 
                    string bookCount = bookData[2].Trim(); 
                    if (bookTitle.Contains(search.Trim().ToLower()) || bookAuthor.Contains(search.Trim().ToLower())) 
                    {
                        txt_allBook.text += $"<color=red>  {bookTitle}  --->   {bookAuthor}  --->   {bookCount}</color>\n"; 
                    }
                }
            }
        }
    //bu fonksiyonda ise kitap almayý saðlýyoruz
        public void BorrowBook()
        {
            string author = input_author.text;
            string book = input_book.text;
            bool found = false;
            List<string> newBookInfoList = new List<string>();
            foreach (string bookInfo in bookInfoArray)
            {
                string[] bookData = bookInfo.Split(',');
                if (bookData.Length >= 3)
                {
                    string bookTitle = bookData[0].Trim().ToLower();
                    string bookAuthor = bookData[1].Trim().ToLower();
                    int bookCount = int.Parse(bookData[2].Trim());
                    if (bookTitle == book.Trim().ToLower() && bookAuthor == author.Trim().ToLower())
                    {
                        if (bookCount > 0)
                        {
                            bookCount -= 1;
                            found = true;
                        }
                        else
                        {
                            borrow_Book.text = "Bu kitap þu anda stokta yok.";

                            return;
                        }
                    }
                    newBookInfoList.Add($"{bookTitle}, {bookAuthor}, {bookCount}");
                }
            }
            if (!found)
            {
                borrow_Book.text = "Bu kitap kütüphanede bulunmuyor.";

                return;
            }
            else
            {
                borrow_Book.text = "";
            }
            File.WriteAllLines(myFilePath, newBookInfoList.ToArray());
            ReadFromTheFile();
        }
    //kitap iadesi saðlanan fonksiyon
        public void ReturnBook()
        {
            string author = input_author.text;
            string book = input_book.text;
            bool found = false;
            List<string> newBookInfoList = new List<string>();
            foreach (string bookInfo in bookInfoArray)
            {
                string[] bookData = bookInfo.Split(',');
                if (bookData.Length >= 3)
                {
                    string bookTitle = bookData[0].Trim().ToLower();
                    string bookAuthor = bookData[1].Trim().ToLower();
                    int bookCount = int.Parse(bookData[2].Trim());
                    if (bookTitle == book.Trim().ToLower() && bookAuthor == author.Trim().ToLower())
                    {
                        bookCount += 1;
                        found = true;
                    }
                    newBookInfoList.Add($"{bookTitle}, {bookAuthor}, {bookCount}");
                }
            }
            if (!found)
            {
                Debug.Log("Bu kitabý iade edemezsiniz, çünkü kütüphanede bulunmuyor.");
                return;
            }
            File.WriteAllLines(myFilePath, newBookInfoList.ToArray());
            ReadFromTheFile();
        }
    //kitap alýþ zamanýný gösteren ve zaman geçtiyse ceza yemesini saðlayan fonksiyon
        public void CheckDueDate()
        {
            txt_allBook.text = ""; 
            DateTime today = DateTime.Now; 
            foreach (string bookInfo in bookInfoArray)
            {
                string[] bookData = bookInfo.Split(','); 
                if (bookData.Length >= 5) 
                {
                    string bookTitle = bookData[0].Trim(); 
                    string bookAuthor = bookData[1].Trim();
                    int bookCount = int.Parse(bookData[2].Trim());
                    DateTime borrowDate = DateTime.Parse(bookData[3].Trim()); 
                    DateTime dueDate = DateTime.Parse(bookData[4].Trim()); 
                    if (dueDate < today) 
                    {
                        int daysLate = (today - dueDate).Days; 
                        double fine = daysLate * 0.5;
                        txt_allBook.text += $"<color=red>  {bookTitle}  --->   {bookAuthor}  --->   Süre {daysLate} gün geçti. Ceza ücreti: {fine} TL</color>\n"; 
                    }
                    else
                    {
                        int daysLate = (today - dueDate).Days;
                        Debug.Log("kitap süreniz þu kadar kaldý 10 ");
                    }
                }
            }
        }
    

    
}


