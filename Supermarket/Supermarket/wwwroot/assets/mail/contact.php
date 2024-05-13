<?php
if(empty($_POST['name']) || empty($_POST['subject']) || empty($_POST['message']) || !filter_var($_POST['email'], FILTER_VALIDATE_EMAIL)) {
  http_response_code(500);
  exit();
}

$name = strip_tags(htmlspecialchars($_POST['name']));
$email = strip_tags(htmlspecialchars($_POST['email']));
$m_subject = strip_tags(htmlspecialchars($_POST['subject']));
$message = strip_tags(htmlspecialchars($_POST['message']));

$to = "vietkhoi998@gmail.com"; // Change this email to your //
$subject = "$m_subject:  $name";
$body = "You have received a new message from your website contact form.\n\n"."Here are the details:\n\nName: $name\n\n\nEmail: $email\n\nSubject: $m_subject\n\nMessage: $message";
$header = "From: $email";
$header .= "Reply-To: $email";	

 if (mail($to, $subject, $body, $headers)) {
        // Trả về mã thành công nếu email được gửi thành công
        http_response_code(200);
        echo "Tin nhắn của bạn đã được gửi.";
    } else {
        // Trả về mã lỗi 500 nếu có lỗi khi gửi email
        http_response_code(500);
        echo "Xin lỗi, có lỗi xảy ra. Vui lòng thử lại sau!";
    }
} else {
    // Trả về mã lỗi 403 nếu yêu cầu không hợp lệ
    http_response_code(403);
    echo "Có lỗi xảy ra với yêu cầu của bạn.";
}
?>
