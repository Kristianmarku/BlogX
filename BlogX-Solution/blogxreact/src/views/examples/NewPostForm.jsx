import React, { useState } from "react";
import {
  Container,
  Row,
  Col,
  Form,
  FormGroup,
  Label,
  Input,
  Button,
} from "reactstrap";
import axios from "axios";
import IndexNavbar from "components/Navbars/IndexNavbar";

const NewPostForm = () => {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [category, setCategory] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Fetch userId from cookies
    const cookieString = document.cookie;
    const userIdCookie = cookieString
      .split("; ")
      .find((cookie) => cookie.startsWith("userId"));
    const userId = userIdCookie ? userIdCookie.split("=")[1] : null;

    try {
      // API request to post a new comment
      const response = await axios.post("https://localhost:7028/api/PostApi", {
        userId,
        title,
        content,
        category_id: category,
      });

      console.log("Post response:", response.data);
      // Reset form fields after successful submission
      setTitle("");
      setContent("");
      setCategory("");
    } catch (error) {
      console.error("Error posting new comment:", error);
    }
  };

  return (
    <Container className="mt-5">
      <IndexNavbar />
      <Row className="justify-content-md-center">
        <Col md="6">
          <h2 className="mb-4">Create a New Post</h2>
          <Form onSubmit={handleSubmit}>
            <FormGroup>
              <Label for="formTitle">Title</Label>
              <Input
                type="text"
                placeholder="Enter post title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            </FormGroup>

            <FormGroup>
              <Label for="formContent">Content</Label>
              <Input
                type="textarea"
                rows="3"
                placeholder="Enter post content"
                value={content}
                onChange={(e) => setContent(e.target.value)}
              />
            </FormGroup>

            <FormGroup>
              <Label for="formCategory">Category</Label>
              <Input
                type="select"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
              >
                <option value="">Select a category</option>
                {/* Add your categories dynamically here */}
                <option value="1">Category 1</option>
                <option value="2">Category 2</option>
                {/* Add more categories as needed */}
              </Input>
            </FormGroup>

            <Button color="primary" type="submit">
              Submit
            </Button>
          </Form>
        </Col>
      </Row>
    </Container>
  );
};

export default NewPostForm;
