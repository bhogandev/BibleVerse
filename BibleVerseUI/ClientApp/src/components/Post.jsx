﻿import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter} from 'reactstrap';
import Cookies from 'universal-cookie';
import axios from 'axios';
import CreateCommentForm from './CreateCommentForm.jsx';
import Comment from './Comment.jsx';
import ConfirmationModal from './ConfirmationModal.jsx';
import { NavDropdown, Container, NavItem, Dropdown, NavLink } from 'react-bootstrap';

class Post extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            photos: null,
            p: null,
            videos: null,
            v: null,
            attachments: this.props.Attachments,
            IsLiked: this.props.IsLiked,
            postUpdate: false,
            likes: this.props.Likes,
            comments: this.props.Comments,
            CExt: this.props.CExt,
            coms: null,
            isOwner: this.props.isOwner,
            showModal: false
        }
        
        

    }

    componentDidMount() {
        //console.log(this.state.CExt);
        if (this.state.CExt != null) {
            var comsState = this.state.CExt;
            const coms = comsState.map(com => {
                return <Comment key={com.Id} CommentUserId={com.CommentUserId} Body={com.Body} CreateDateTime={com.CreateDateTime} />
            });

            this.setState({ coms: coms });
        }


        if (this.props.Attachments != null) {
            var p = [];
            var v = [];

            for (var i = 0; i < this.state.attachments.length; i++) {
                if (this.state.attachments[i]['ContentType'] == "Photo") {
                    p.push(this.state.attachments[i]);
                } else {
                    v.push(this.state.attachments[i]);
                }

            }
            this.setState({ photos: p, videos: v });


            if (p != [] || v != []) {
                const photos = p.map(attachment => {
                    return (
                        <img key={attachment['AttachmentID']} src={attachment["Link"]} />
                    )
                });

                this.setState({ p: photos });
   
                const videos = v.map(attachment => {
                    return (
                        <video key={attachment['AttachmentID']} src={attachment["Link"]} control>
                            Your browser does not support the video tag.
                        </video>
                        )
                })
                

            }
        }
    }

    componentDidUpdate() {
        if (this.state.postUpdate) {
            this.UpdatePost();
            this.setState({ postUpdate: false });
        }

        
    }

    openModal() {
        //!this.state.showModal ? this.setState({ showModal: true }) : this.setState({ showModal: false });
        this.setState({ showModal: !this.state.showModal });
    }


    renderDelete(isOwner) {
        if (isOwner) {
            return (<NavDropdown.Item onClick={() => this.openDeleteModal()}>Delete post</NavDropdown.Item>);
        } else {
            return "";
        }
    }

    async Like(IsLiked) {
        var counter = this.state.likes;
        var invertLike;

        if (IsLiked == "Like") {
            counter++;
            invertLike = "Unlike";
        } else {
            counter--;
            invertLike = "Like";
        }
        this.setState({ likes: counter, IsLiked: invertLike });

        const cookies = new Cookies();

        this.setState({ btnDisabled: true });

        try {

            let res = await (await fetch("https://localhost:44307/api/Post/Interact", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': cookies.get('token'),
                    'IntType': IsLiked
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    ParentId: this.props.PostId,
                    LikeType: IsLiked
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());
                if (IsLiked == "Like") {
                    counter--;
                    this.setState({ IsLiked: "Like" });
                } else {
                    counter++;
                    this.setState({ IsLiked: "Unlike" });
                }
                this.setState({ likes: counter });
            } else {
            }
        } catch (exception) {
            if (IsLiked == "Like") {
                counter--;
                this.setState({ IsLiked: "Like" });
            } else {
                counter++;
                this.setState({ IsLiked: "Unlike" });
            }
            this.setState({ likes: counter });
            console.log(exception);
        }


    }

     openDeleteModal() {
         this.openModal();
     }

    async DeletePost(postId) {
        this.openModal();
        /*
         * Write logic here to make post delete call.
         * Pass token and verify on API side user is correct user via token
         */

        var cookies = new Cookies();

        try {

            let res = await (await fetch("https://localhost:44307/api/Post/DeletePost", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': cookies.get('token'),
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    PostId: postId,
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                this.props.GetTL();
            } else if (res && res.status == "409") {
                //Handle Error
            } else {
                //General Error Message
            }
        } catch (exception) {
            //Do something with exception
            console.log(exception);
        }

    }

    render() {
        return (
            <Container>
                <span><a href="#"><b>{this.props.Username}</b></a></span> <span>{this.props.CreateDateTime}</span>
                {/*Get rid of dropdown arrow at some point (apply custom component)*/}
                <NavDropdown title="..." id="basic-nav-dropdown">
                    <NavDropdown.Item>Get notified about this post</NavDropdown.Item>
                    <NavDropdown.Item>Hide post</NavDropdown.Item>
                    <NavDropdown.Item>Report post</NavDropdown.Item>
                    {this.renderDelete(this.state.isOwner)}
                </NavDropdown>
                <ConfirmationModal Body="Are you sure you would like to delete this post?" OnClickFirst={() => this.DeletePost(this.props.PostId)} OnClickSecond={() => this.openModal()} isOpen={this.state.showModal} />
                <br />
                {this.state.p}
                {this.state.v}
                <p>{this.props.Body}</p>
                <span>{this.state.likes} Likes</span> <span>{this.props.Comments} Comments</span>
                <br />
                {this.state.coms}
                <Button onClick={() => this.Like(this.state.IsLiked)}>{this.state.IsLiked}</Button>
                <CreateCommentForm PostId={this.props.PostId}/>
            </Container>
            )
    }
}
export default Post;