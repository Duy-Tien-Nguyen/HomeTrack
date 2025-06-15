import img1 from "../../assets/imgs/anh-mang.jpg";

export const listReport = [
  {
    img: img1,
    backGroud: "#fef3c7",
    color: "#92400e",
    report: "Sai tag",
    id: "12345",
    content:
      'Hình ảnh này được gắn tag không chính xác. Đây không phải là "bàn làm việc" mà là "bàn ăn".',
    reportBy: "user123",
    date: "15/06/2023",
  },
  {
    img: img1,

    report: "nội dung không phù hợp",
    backGroud: "#fee2e2",
    color: "#B91c1c",
    id: "12346",
    content:
      "Hình ảnh này chứa nội dung không phù hợp với quy định của hệ thống.",
    reportBy: "user456",
    date: "15/06/2023",
  },
];
